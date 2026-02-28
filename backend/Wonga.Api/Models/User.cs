namespace Wonga.Api.Models;

public class User
{
    public int UserID {get; set;}
    public string Email {get; set;} = string.Empty;
	public string FirstName {get; set;} = string.Empty;
	public string LastName {get; set;} = string.Empty;
	
    // Storing the password's salted hash as a base64 string
    public string PasswordHash {get; set;} = string.Empty;
    
	/* 
	TODO: Add additional properies to this User class.
		public DateTime CreatedAt {get; set;}
		public DateTime UpdateAt {get; set;}
		public DateTime LastLogin {get; set;} 
		public bool Deleted {get; set;}
	*/
}