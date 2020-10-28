// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using AutoMapper;
using Gameboard.Data;
using Gameboard.Data.Repositories;
using Gameboard.ViewModels;
using GameEngine.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Stack.Http.Exceptions;
using Stack.Http.Identity;
using Stack.Patterns.Service.Models;
using Stack.Validation.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gameboard.Services
{
    /// <summary>
    /// game service
    /// </summary>
    public class GameService : Service<IGameRepository, Game>
    {           
        IGameEngineService GameEngineService { get; }

        public GameService(
            IStackIdentityResolver identityResolver,
            IGameRepository repository,
            IMapper mapper,
            IValidationHandler validationHandler,            
            IGameFactory gameFactory,
            IGameEngineService gameEngineService,
            LeaderboardOptions leaderboardOptions)
            : base(identityResolver, repository, mapper, validationHandler, gameFactory, leaderboardOptions)
        {
            GameEngineService = gameEngineService;
        }

        /// <summary>
        /// get all games
        /// </summary>
        /// <param name="dataFilter"></param>
        /// <returns></returns>
        public async Task<PagedResult<Game, GameDetail>> GetAll(GameDataFilter dataFilter)
        {
            if (!Identity.User.IsGameDesigner)
                throw new EntityPermissionException("Action not allowed.");

            return await PagedResultFactory.ExecuteAsync<Game, GameDetail>(Repository.GetAll(), dataFilter, Identity, GetMappingOperationOptions());
        }

        /// <summary>
        /// get all games
        /// </summary>
        public async Task<GameDetail> GetById(string id)
        {
            if (!Identity.User.IsGameDesigner)
                throw new EntityPermissionException("Action not allowed.");

            var game = await Repository.GetAll()
                .SingleOrDefaultAsync(g => g.Id.Trim().ToLower() == id.Trim().ToLower());

            return Map<GameDetail>(game);
        }

        /// <summary>
        /// create game
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GameDetail> Create(GameEdit model)
        {
            if (!Identity.User.IsGameDesigner)
                throw new EntityPermissionException("Action not allowed.");

            await ValidationHandler.ValidateRulesFor(model);

            var game = Map<Game>(model);

            await Repository.Add(game);

            await Refresh();

            return await GetById(game.Id);
        }

        /// <summary>
        /// update game
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GameDetail> Update(string id, GameEdit model)
        {
            if (!Identity.User.IsGameDesigner)
                throw new EntityPermissionException("Action not allowed.");

            await ValidationHandler.ValidateRulesFor(model);

            var game = await Repository.GetById(id);
            
            game.EnrollmentEndsAt = model.EnrollmentEndsAt;
            game.MaxConcurrentProblems = model.MaxConcurrentProblems;
            game.MinTeamSize = model.MinTeamSize;
            game.MaxTeamSize = model.MaxTeamSize;
            game.Name = model.Name;
            game.Slug = model.Id;
            game.StartTime = model.StartTime;
            game.StopTime = model.StopTime;

            UpdateBoards(model, game);

            await Repository.Update(game);
            await Refresh();

            return await GetById(game.Id);
        }

        void UpdateBoards(GameEdit model, Game game)
        {
            var updateBoardIds = model.Boards.Select(b => b.Id);
            var removeBoards = game.Boards.Where(b => !updateBoardIds.Contains(b.Id) && !string.IsNullOrWhiteSpace(b.Id));

            int order = 0;
            foreach (var board in model.Boards)
            {
                var eb = game.Boards.SingleOrDefault(b => b.Id == board.Id);

                if (eb == null)
                {
                    eb = new Board { Id = Guid.NewGuid().ToString(), GameId = game.Id };
                    game.Boards.Add(eb);
                    Repository.DbContext.Boards.Add(eb);
                }

                eb.Name = board.Name;
                eb.Badges = board.Badges;
                eb.StartText = board.StartText;
                eb.StartTime = board.StartTime;
                eb.StopTime = board.StopTime;
                eb.RequiredBadges = board.RequiredBadges;
                eb.MaxSubmissions = board.MaxSubmissions;
                eb.MaxMinutes = board.MaxMinutes;
                eb.BoardType = board.BoardType;
                eb.IsPreviewAllowed = board.IsPreviewAllowed;
                eb.IsPractice = board.IsPractice;
                eb.IsResetAllowed = board.IsResetAllowed;
                eb.IsTitleVisible = board.IsTitleVisible;
                eb.AllowSharedWorkspaces = board.AllowSharedWorkspaces;
                eb.Order = order;
                eb.CertificateThreshold = board.CertificateThreshold;
                eb.MaxConcurrentProblems = board.MaxConcurrentProblems;

                if (eb.BoardType == BoardType.Trivia)
                {
                    eb.Maps.Clear();
                    UpdateCategories(board, eb);
                }

                if (eb.BoardType == BoardType.Map)
                {
                    eb.Categories.Clear();
                    UpdateMaps(board, eb);
                }

                order++;
            }

            if (removeBoards.Any())
            {
                Repository.DbContext.Boards.RemoveRange(removeBoards);
            }
        }

        void UpdateCategories(BoardEdit board, Board eb)
        {
            var updateCategoryIds = board.Categories.Select(c => c.Id);
            var removeCategories = eb.Categories.Where(c => !updateCategoryIds.Contains(c.Id) && !string.IsNullOrWhiteSpace(c.Id));

            int order = 0;
            foreach (var category in board.Categories)
            {
                var ec = eb.Categories.SingleOrDefault(c => c.Id == category.Id);

                if (ec == null)
                {
                    ec = new Category { Id = Guid.NewGuid().ToString(), BoardId = eb.Id };
                    eb.Categories.Add(ec);
                    Repository.DbContext.Categories.Add(ec);
                }

                ec.Name = category.Name;
                ec.Order = order;
                order++;
                UpdateQuestions(category, ec);
            }

            if (removeCategories.Any())
            {
                Repository.DbContext.Categories.RemoveRange(removeCategories);
            }
        }

        void UpdateMaps(BoardEdit board, Board eb)
        {
            var updateMapIds = board.Maps.Select(c => c.Id);
            var removeMaps = eb.Maps.Where(m => !updateMapIds.Contains(m.Id) && !string.IsNullOrWhiteSpace(m.Id));

            int order = 0;
            foreach (var map in board.Maps)
            {
                var em = eb.Maps.SingleOrDefault(c => c.Id == map.Id);

                if (em == null)
                {
                    em = new Map {
                        Id = string.IsNullOrWhiteSpace(map.Id) ? Guid.NewGuid().ToString() : map.Id,
                        BoardId = eb.Id
                    };
                    eb.Maps.Add(em);
                    Repository.DbContext.Maps.Add(em);
                }

                em.ImageUrl = map.ImageUrl;
                em.Name = map.Name;
                em.Order = order;

                order++;

                UpdateCoordinates(map, em);
            }

            if (removeMaps.Any())
            {
                Repository.DbContext.Maps.RemoveRange(removeMaps);
            }
        }

        void UpdateQuestions(CategoryEdit category, Category ec)
        {
            var updateQuestionIds = category.Questions.Select(c => c.Id);
            var removeQuestions = ec.Questions.Where(q => !updateQuestionIds.Contains(q.Id) && !string.IsNullOrWhiteSpace(q.Id));

            int order = 0;
            foreach (var question in category.Questions)
            {
                var eq = ec.Questions.SingleOrDefault(c => c.Id == question.Id);

                var isNew = false;

                if (eq == null)
                {
                    eq = new Question { Id = Guid.NewGuid().ToString(), CategoryId = ec.Id };
                    isNew = true;
                }

                eq.Points = question.Points;
                eq.Order = order;
                eq.ChallengeLink.Id = string.IsNullOrWhiteSpace(question.ChallengeLink.Id) ? Guid.NewGuid().ToString() : question.ChallengeLink.Id;
                eq.ChallengeLink.Slug = question.ChallengeLink.Slug;
                eq.IsDisabled = question.IsDisabled;

                order++;

                if (isNew) {
                    ec.Questions.Add(eq);
                    Repository.DbContext.Questions.Add(eq);
                }
            }

            if (removeQuestions.Any())
            {
                Repository.DbContext.Questions.RemoveRange(removeQuestions);
            }
        }

        void UpdateCoordinates(MapEdit model, Map entity)
        {
            var ids = model.Coordinates.Select(c => c.Id);
            var remove = entity.Coordinates.Where(c => !ids.Contains(c.Id) && !string.IsNullOrWhiteSpace(c.Id));

            foreach (var coordinate in model.Coordinates)
            {
                var ec = entity.Coordinates.SingleOrDefault(c => c.Id == coordinate.Id);

                var isNew = false;
                if (ec == null)
                {
                    ec = new Coordinate { Id = Guid.NewGuid().ToString(), MapId = entity.Id };
                    isNew = true;
                }

                ec.Name = coordinate.Name;
                ec.ActionType = coordinate.ActionType;
                ec.ActionValue = coordinate.ActionValue;
                ec.Points = coordinate.Points;
                ec.X = coordinate.X;
                ec.Y = coordinate.Y;
                ec.Radius = coordinate.Radius;
                ec.IsDisabled = coordinate.IsDisabled;

                if (ec.ActionType == ActionType.Challenge)
                {
                    ec.ChallengeLink = new ChallengeLink
                    {
                        Id = string.IsNullOrWhiteSpace(coordinate.ChallengeLink.Id) ? Guid.NewGuid().ToString() : coordinate.ChallengeLink.Id,
                        Slug = coordinate.ChallengeLink.Slug
                    };
                }
                else 
                {
                    ec.ChallengeLink = new ChallengeLink();
                }

                if (isNew) 
                {
                    entity.Coordinates.Add(ec);
                    Repository.DbContext.Coordinates.Add(ec);
                }
            }

            if (remove.Any())
            {
                Repository.DbContext.Coordinates.RemoveRange(remove);
            }
        }

        public async Task Refresh()
        {
            if (!Identity.User.IsGameDesigner)
                throw new EntityPermissionException("Action not allowed.");

            await GameFactory.Refresh();
        }

        /// <summary>
        /// get game surveys
        /// </summary>
        /// <param name="boardId"></param>
        /// <returns></returns>
        public async Task<SurveyReport> GetSurveyReport()
        {
            var game = GameFactory.GetGame();           

            var users = await DbContext.Users
                .Include(u => u.Team)
                .Where(s => !string.IsNullOrWhiteSpace(s.Survey))
                .ToListAsync();

            var result = new SurveyReport
            {
                GameId = game.Id,
                GameName = game.Name
            };

            foreach (var user in users)
            {
                var responses = JsonConvert.DeserializeObject<List<SurveyResponse>>(user.Survey);

                foreach (var response in responses)
                {
                    var item = new SurveyItem
                    {
                        Date = DateTime.UtcNow,                        
                        Question = response.Text,
                        Answer = response.Value
                    };

                    result.Items.Add(item);
                }
            }

            result.Items = result.Items
                .OrderBy(r => r.Date)
                .ThenBy(r => r.Question)
                .ToList();

            return result;
        }
    }
}

