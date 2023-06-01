using MessagePack;
using System.ComponentModel.DataAnnotations;

namespace MyNutritionist.Models
{
    public abstract class Person
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int PID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
            
        public Person() { }

        /*public Person(int pID, string name, string email, string username, string password)
        {
            PID = pID;
            Name = name;
            Email = email;
            Username = username;
            Password = password;
        }*/
    }
}
