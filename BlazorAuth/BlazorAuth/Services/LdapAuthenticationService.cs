using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;
using System.Collections.Generic;

namespace BlazorAuth.Services
{
    public class LdapAuthenticationService
    {
        private readonly IConfiguration _configuration;

        public LdapAuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Authenticate(string username, string password)
        {
            try
            {
                using (var ldapConnection = new LdapConnection())
                {
                    ldapConnection.Connect(_configuration["LdapConfig:Server"], _configuration.GetValue<int>("LdapConfig:Port"));
                    var userDn = GetUserDn(username);
                    if (string.IsNullOrEmpty(userDn))
                    {
                        return false; // User DN not found
                    }

                    ldapConnection.Bind(userDn, password);
                    return ldapConnection.Bound;
                }
            }
            catch (LdapException)
            {
                // Handle exceptions
                return false;
            }
        }

        public IEnumerable<string> GetUserRoles(string username)
        {
            var roles = new List<string>();
            
            
            switch (username.ToLower())
            {
                case "user1":
                    roles.Add("Administrator");
                    break;
                case "user2":
                    roles.Add("User1");
                    break;
                case "user3":
                    roles.Add("User2");
                    break;
                default:
                    roles.Add("UNAUTHORIZED"); 
                    break;
            }
            return roles;
        }

        private string GetUserDn(string username)
        {
            return $"cn={username},ou=users,dc=nodomain";
        }
    }
}
