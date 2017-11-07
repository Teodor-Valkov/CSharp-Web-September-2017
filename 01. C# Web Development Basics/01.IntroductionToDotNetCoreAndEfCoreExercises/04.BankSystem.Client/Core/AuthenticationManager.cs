namespace _04.BankSystem.Client.Core
{
    using _04.BankSystem.Models;
    using System;

    public static class AuthenticationManager
    {
        private static User currentUser;

        // Checks if there is currently logged in user.
        public static bool IsAuthenticated()
        {
            return currentUser != null;
        }

        // Get currently logged in user.
        public static User GetCurrentUser()
        {
            return currentUser;
        }

        // Log in current user.
        public static void Login(User user)
        {
            if (IsAuthenticated())
            {
                throw new InvalidOperationException("You should logout first!");
            }

            if (user == null)
            {
                throw new InvalidOperationException("User to log in is invalid!");
            }

            currentUser = user;
        }

        // Logout current user.
        public static void Logout()
        {
            if (!IsAuthenticated())
            {
                throw new InvalidOperationException("You should login first!");
            }

            currentUser = null;
        }
    }
}