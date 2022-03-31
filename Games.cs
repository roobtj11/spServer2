using System.Text;

public class Games
{
    int GameID;
    bool complete;
    string winner;
    int current_set;
    string t1_Name;
    string t2_Name;
    int[] t1_Scores = { };
    int[] t2_Scores = { };
    int t1s1_Score;
    int t1s2_Score;
    int t1s3_Score;
    int t2s1_Score;
    int t2s2_Score;
    int t2s3_Score;
    int t1_SetsWon;
    int t2_SetsWon;

    public Games(int GameID, bool complete, string winner,int current_set, string t1_Name, string t2_Name, int t1s1_Score, int t1s2_Score, int t1s3_Score, int t2s1_Score, int t2s2_Score, int t2s3_Score, int t1_SetsWon, int t2_SetsWon)
    {
        this.GameID = GameID;
        this.complete = complete;
        this.t1_Name = t1_Name;
        this.t2_Name = t2_Name;

        t1_Scores[0] = 0;
        t2_Scores[0] = 0;

        if (complete == true)
        {
            this.winner = winner;
            this.current_set = 0;
            this.t1_Scores[1] = t1s1_Score;
            this.t2_Scores[1] = t2s1_Score;
            this.t1_Scores[2] = t1s2_Score;
            this.t2_Scores[2] = t2s2_Score;
            this.t1_Scores[3] = t1s3_Score;
            this.t2_Scores[3] = t2s3_Score;
            this.t1_SetsWon = t1_SetsWon;
            this.t2_SetsWon = t2_SetsWon;
        }
        else
        {
            this.winner = string.Empty;
            this.current_set = current_set;
            if (current_set >= 1) {
                this.t1_Scores[1] = t1s1_Score;
                this.t2_Scores[1] = t2s1_Score;
                if (current_set >= 2)
                {
                    this.t1_Scores[2] = t1s2_Score;
                    this.t2_Scores[2] = t2s2_Score;
                    if (current_set == 3)
                    {
                        this.t1_Scores[3] = t1s3_Score;
                        this.t2_Scores[3] = t2s3_Score;

                    }
                    else
                    {
                        this.t1_Scores[3] = 0;
                        this.t2_Scores[3] = 0;
                    }
                }
                else
                {
                    this.t1_Scores[2] = 0;
                    this.t2_Scores[2] = 0;
                    this.t1_Scores[3] = 0;
                    this.t2_Scores[3] = 0;
                }
            }
            this.t1_SetsWon = t1_SetsWon;
            this.t2_SetsWon = t2_SetsWon;
        }
    }

    public void addPoint(string teamname) //1 or 2
    {
        if (teamname == this.t1_Name)
        {
            t1_Scores[current_set] = ++t1_Scores[current_set];
        }
        else
        {
            t2_Scores[current_set] = ++t2_Scores[current_set];
        }
    }
    public void removePoint(string teamname)
    {
        if (teamname == t1_Name)
        {
            t1_Scores[current_set] = --t1_Scores[current_set];
        }
        else
        {
            t2_Scores[current_set] = --t2_Scores[current_set];
        }
    }

    public void nextSet()
    {
        if(t1_Scores[current_set] > t2_Scores[current_set])
        {
            t1_SetsWon++;
            if (t1_SetsWon == 2)
            {
                winner = t1_Name;
                current_set = 0;
                complete = true;
                return;
            }
        }
        else
        {
            t2_SetsWon++;
            if (t1_SetsWon == 2)
            {
                winner = t2_Name;
                current_set = 0;
                complete = true;
                return;
            }
        }
        current_set++;
    }
    public int getGameId()
    {
        return GameID;
    }
    public bool getComplete()
    {
        return complete;
    }

}