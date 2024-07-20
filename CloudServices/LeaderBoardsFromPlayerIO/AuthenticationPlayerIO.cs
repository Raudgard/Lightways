using System.Collections.Generic;
using UnityEngine;
using PlayerIOClient;
using Saving;

namespace CloudServices.LeaderBoardsFromPlayerIO
{
    public static class AuthenticationPlayerIO
    {
        private static readonly string gameID = "lightways-2gb6s8ps5esvppcddqnswa";
        private static readonly string sharedSecret = "QkwQKfLfjfpggAKfKafhFHdjdK";

        /// <summary>
        /// Метод аутентификации пользователя на PlayerIO и получения его клиента.
        /// На коллбэки нельзя ставить аргументами null! Взамен этого можно поставить пустой делегат (например, delegate { }).
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callbackClient"></param>
        /// <param name="callbackError"></param>
        public static void AuthenticationUser(string userId, Callback<Client> callbackClient, Callback<PlayerIOError> callbackError)
        {
            string authString = PlayerIO.CalcAuth256(userId, sharedSecret);

            PlayerIO.Authenticate(
                gameID,            //Your game id
                "public",                               //Your connection id
                new Dictionary<string, string> {        //Authentication arguments
                { "userId", userId },
                { "auth", authString },
                },
                null,                                   //PlayerInsight segments
                callbackClient,
                callbackError
            );
        }
    }
}
