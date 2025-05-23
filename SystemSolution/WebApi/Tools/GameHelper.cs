﻿using WebApi.DTOs;
using WebApi.Models;

namespace WebApi.Tools
{
    public static class GameHelper
    {
        public static GameWithIdDTO SetGameWithIdDTOFromGame(Game game)
        {
            return new GameWithIdDTO
            {
                Id = game.Id,
                Title = game.Title ?? "",
                Description = game.Description,
                DevTeam = game.DevTeam ?? "",
                VoteCount = game.Votes.Count
            };
        }
        public static GameDTO SetGameDTOFromGame(Game game)
        {
            return new GameDTO
            {
                Title = game.Title ?? "",
                Description = game.Description,
                DevTeam = game.DevTeam ?? "",
                VoteCount = game.Votes.Count
            };
        }
        public static Game SetGameFromGameDTO(GameWithIdDTO gameDTO)
        {
            return new Game
            {
                Title = gameDTO.Title,
                Description = gameDTO.Description,
                DevTeam = gameDTO.DevTeam
            };
        }
        public static Game SetGameFromGameDTO(GameDTO gameDTO)
        {
            return new Game
            {
                Title = gameDTO.Title,
                Description = gameDTO.Description,
                DevTeam = gameDTO.DevTeam
            };
        }
    }
}
