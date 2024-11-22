using Microsoft.Azure.Cosmos.Table;
namespace PROG3.Models
{
    public class UserEntity : TableEntity
    {
        public string Username { get; set; }
        public string Password { get; set; } 
        public UserEntity(string username)
        {
            PartitionKey = "UserPartition"; 
            RowKey = username;  
        }

        public UserEntity() { }
    }

}
