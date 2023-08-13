using NotifyBotApi.Interfaces;

namespace NotifyBotApi.Services
{
    public class ChatService
    {
        public static Dictionary<string, LinkedList<string>> UsersOnline = new Dictionary<string, LinkedList<string>>();

        public bool AddUserOnline(string groupName, string userName)
        {
            lock (UsersOnline)
            {
                if(UsersOnline.ContainsKey(groupName) == false)
                {
                    UsersOnline.Add(groupName, new LinkedList<string>());
                    UsersOnline[groupName].AddLast(userName);
                    return true;
                } 

                var user = UsersOnline[groupName];
                if(user.Contains(userName))
                {
                    return true;
                }
                UsersOnline[groupName].AddLast(userName);
                return true;
            }
        }

        public bool RemoveUserOnline(string groupName, string userName)
        {
            lock (UsersOnline)
            {
                if (UsersOnline.ContainsKey(groupName))
                {
                    UsersOnline[groupName].Remove(userName);
                    return true;
                }
                return false;
            }
        }

        public LinkedList<string> GetUsersOnlineAGroup(string groupName) 
        { 
            lock(UsersOnline)
            {
                if( UsersOnline.ContainsKey(groupName))
                {
                    return UsersOnline[groupName];
                }
                return new LinkedList<string>();
            }
        }
    }
}
