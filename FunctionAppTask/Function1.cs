using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FunctionAppTask
{
    public static class Function1
    {
        private static List<Player> players = new List<Player>();

        [FunctionName("GetById")]
        public static IActionResult GetById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "players/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            if (!int.TryParse(id, out int playerId))
            {
                return new BadRequestObjectResult("Invalid player ID.");
            }

            var player = players.Find(p => p.PlayerId == playerId);
            if (player == null)
            {
                return new NotFoundObjectResult($"Player with ID {playerId} not found.");
            }
            return new OkObjectResult(player);
        }

        [FunctionName("GetAll")]
        public static IActionResult GetAll(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "players")] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult(players);
        }

        [FunctionName("GetByTeamId")]
        public static IActionResult GetByTeamId(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "teams/{teamId}/players")] HttpRequest req,
            ILogger log, string teamId)
        {
            if (!int.TryParse(teamId, out int teamIdValue))
            {
                return new BadRequestObjectResult("Invalid team ID.");
            }

            var teamPlayers = players.FindAll(p => p.TeamId == teamIdValue);
            return new OkObjectResult(teamPlayers);
        }

        [FunctionName("Add")]
        public static IActionResult Add(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "players")] HttpRequest req,
            ILogger log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var newPlayer = JsonConvert.DeserializeObject<Player>(requestBody);
            players.Add(newPlayer);
            return new OkObjectResult(newPlayer);
        }

        [FunctionName("Update")]
        public static IActionResult Update(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "players/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            if (!int.TryParse(id, out int playerId))
            {
                return new BadRequestObjectResult("Invalid player ID.");
            }

            var playerToUpdate = players.Find(p => p.PlayerId == playerId);
            if (playerToUpdate == null)
            {
                return new NotFoundObjectResult($"Player with ID {playerId} not found.");
            }

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            var updatedPlayer = JsonConvert.DeserializeObject<Player>(requestBody);
            playerToUpdate.Name = updatedPlayer.Name;
            playerToUpdate.Surname = updatedPlayer.Surname;
            playerToUpdate.Score = updatedPlayer.Score;
            playerToUpdate.TeamId = updatedPlayer.TeamId;

            return new OkObjectResult(playerToUpdate);
        }

        [FunctionName("Delete")]
        public static IActionResult Delete(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "players/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            if (!int.TryParse(id, out int playerId))
            {
                return new BadRequestObjectResult("Invalid player ID.");
            }

            var playerToDelete = players.Find(p => p.PlayerId == playerId);
            if (playerToDelete == null)
            {
                return new NotFoundObjectResult($"Player with ID {playerId} not found.");
            }

            players.Remove(playerToDelete);
            return new OkObjectResult($"Player with ID {playerId} deleted successfully.");
        }
    }
}
