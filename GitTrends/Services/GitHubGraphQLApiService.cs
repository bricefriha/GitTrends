﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using GitTrends.Mobile.Common;
using GitTrends.Mobile.Common.Constants;
using GitTrends.Shared;
using Refit;
using Xamarin.Essentials.Interfaces;

namespace GitTrends
{
    public class GitHubGraphQLApiService : BaseMobileApiService
    {
        readonly static Lazy<IGitHubGraphQLApi> _githubApiClientHolder = new Lazy<IGitHubGraphQLApi>(() => RestService.For<IGitHubGraphQLApi>(CreateHttpClient(GitHubConstants.GitHubGraphQLApi)));

        readonly GitHubUserService _gitHubUserService;

        public GitHubGraphQLApiService(IAnalyticsService analyticsService, IMainThread mainThread, GitHubUserService gitHubUserService) : base(analyticsService, mainThread)
        {
            _gitHubUserService = gitHubUserService;
        }

        static IGitHubGraphQLApi GitHubApiClient => _githubApiClientHolder.Value;

        public async Task<(string login, string name, Uri avatarUri)> GetCurrentUserInfo(CancellationToken cancellationToken)
        {
            var token = await _gitHubUserService.GetGitHubToken().ConfigureAwait(false);
            var data = await ExecuteGraphQLRequest(() => GitHubApiClient.ViewerLoginQuery(new ViewerLoginQueryContent(), GetGitHubBearerTokenHeader(token)), cancellationToken).ConfigureAwait(false);

            return (data.Viewer.Alias, data.Viewer.Name, data.Viewer.AvatarUri);
        }

        public async Task<Repository> GetRepository(string repositoryOwner, string repositoryName, CancellationToken cancellationToken)
        {
            var token = await _gitHubUserService.GetGitHubToken().ConfigureAwait(false);
            var data = await ExecuteGraphQLRequest(() => GitHubApiClient.RepositoryQuery(new RepositoryQueryContent(repositoryOwner, repositoryName), GetGitHubBearerTokenHeader(token)), cancellationToken).ConfigureAwait(false);

            return data.Repository;
        }

        public async IAsyncEnumerable<StarGazers> GetStarGazerInfo(string repositoryName, string repositoryOwner, [EnumeratorCancellation] CancellationToken cancellationToken, int numberOfStarGazersPerRequest = 100)
        {
            StarGazerResponse? starGazerResponse = null;

            var token = await _gitHubUserService.GetGitHubToken().ConfigureAwait(false);

            do
            {
                var endCursor = GetEndCursorString(starGazerResponse?.Repository.StarGazers.StarredAt.LastOrDefault()?.Cursor);
                starGazerResponse = await ExecuteGraphQLRequest(() => GitHubApiClient.StarGazerQuery(new StarGazerQueryContent(repositoryName, repositoryOwner, endCursor, numberOfStarGazersPerRequest), GetGitHubBearerTokenHeader(token)), cancellationToken).ConfigureAwait(false);

                if (starGazerResponse?.Repository.StarGazers != null)
                    yield return starGazerResponse.Repository.StarGazers;

            } while (starGazerResponse?.Repository.StarGazers.StarredAt.Count == numberOfStarGazersPerRequest);
        }

        public async IAsyncEnumerable<Repository> GetRepositories(string repositoryOwner, [EnumeratorCancellation] CancellationToken cancellationToken, int numberOfRepositoriesPerRequest = 100)
        {
            if (_gitHubUserService.IsDemoUser)
            {
                //Yield off of main thread to generate the demoDataList
                await Task.Yield();

                for (int i = 0; i < DemoDataConstants.RepoCount; i++)
                {
                    var demoRepo = new Repository($"Repository " + DemoDataConstants.GetRandomText(), DemoDataConstants.GetRandomText(), DemoDataConstants.GetRandomNumber(),
                                                DemoUserConstants.Alias, _gitHubUserService.AvatarUrl,
                                                DemoDataConstants.GetRandomNumber(), _gitHubUserService.AvatarUrl, false, DateTimeOffset.UtcNow);
                    yield return demoRepo;
                }

                //Allow UI to update
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
            else
            {
                RepositoryConnection? repositoryConnection = null;

                do
                {
                    repositoryConnection = await GetRepositoryConnection(repositoryOwner, repositoryConnection?.PageInfo?.EndCursor, cancellationToken, numberOfRepositoriesPerRequest).ConfigureAwait(false);

                    foreach (var repository in repositoryConnection.RepositoryList)
                    {
                        yield return new Repository(repository.Name, repository.Description, repository.ForkCount, repository.Owner.Login, repository.Owner.AvatarUrl,
                                                        repository.Issues.IssuesCount, repository.Url.ToString(), repository.IsFork, repository.DataDownloadedAt);
                    }
                }
                while (repositoryConnection?.PageInfo?.HasNextPage is true);
            }
        }

        static string GetEndCursorString(string? endCursor) => string.IsNullOrWhiteSpace(endCursor) ? string.Empty : "after: \"" + endCursor + "\"";

        async Task<RepositoryConnection> GetRepositoryConnection(string repositoryOwner, string? endCursor, CancellationToken cancellationToken, int numberOfRepositoriesPerRequest = 100)
        {
            var token = await _gitHubUserService.GetGitHubToken().ConfigureAwait(false);
            var data = await ExecuteGraphQLRequest(() => GitHubApiClient.RepositoryConnectionQuery(new RepositoryConnectionQueryContent(repositoryOwner, GetEndCursorString(endCursor), numberOfRepositoriesPerRequest), GetGitHubBearerTokenHeader(token)), cancellationToken).ConfigureAwait(false);

            return data.GitHubUser.RepositoryConnection;
        }

        async Task<T> ExecuteGraphQLRequest<T>(Func<Task<GraphQLResponse<T>>> action, CancellationToken cancellationToken, int numRetries = 2, [CallerMemberName] string callerName = "")
        {
            var response = await AttemptAndRetry_Mobile(action, cancellationToken, numRetries, callerName: callerName).ConfigureAwait(false);

            if (response.Errors != null && response.Errors.Count() > 1)
                throw new AggregateException(response.Errors.Select(x => new Exception(x.Message)));
            else if (response.Errors != null && response.Errors.Any())
                throw new Exception(response.Errors.First().Message.ToString());

            return response.Data;
        }
    }
}
