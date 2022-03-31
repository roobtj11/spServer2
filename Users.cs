using System.Text;

public class Users
{
    private string username;
    private string password_hash;
    private string account_type; // 1=standard 2 = Admin 3 = GOD

    public Users(string username, string password_hash, string account_type)
    {
        this.username = username;
        this.password_hash = password_hash;
        this.account_type = account_type;
    }

    public Users(string username, string password_hash)
    {
        this.username = username;
        this.password_hash = password_hash;
        this.account_type = "1";
    }

    public string getUsername()
    {
        return username;
    }
    public string getPasswordHash()
    {
        return password_hash;
    }
    public string getAccountType()
    {
        return account_type;
    }

    public string printAccountName()
    {
        if (account_type == "2")
            return "Admin";
        else if (account_type == "3")
            return "GOD";
        else
            return "standard";
    }
    public int printAccountPerm()
    {
        if (account_type == "1")
            return 1;
        else if (account_type == "2")
        {
            return 2;
        }
        else
            return 3;
    }

    public string print()
    {
        return password_hash + "," + account_type;
    }

}