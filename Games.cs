using System.Text;
using System.Text.Json;

public class Games
{
    public int GameNum { get; set; }
    public string[] teams { get; set; } = new string[2];
    public bool GameOver { get; set; }
    public string Winner { get; set; }
    public int CurrentSet { get; set; }
    public int[] t1Scores { get; set; } = new int[3];
    public int[] t2Scores { get; set; } = new int[3];
    public int t1SetsWon { get; set; }
    public int t2SetsWon { get; set; }

    public Games(int GameNum, string team1, string team2, bool GameOver, string Winner, int CurrentSet, int t1s1Score, int t1s2Score, int t1s3Score, int t2s1Score, int t2s2Score, int t2s3Score, int t1SetsWon, int t2SetsWon)
    {
        this.GameNum = GameNum;
        teams[0] = team1;
        teams[1] = team2;
        this.GameOver = GameOver;
        this.Winner = Winner;
        if (this.GameOver)
        {
            this.CurrentSet = 69;
        }
        else
        {
            this.CurrentSet = CurrentSet;
        }

        t1Scores[0] = t1s1Score;
        t2Scores[0] = t2s1Score;
        if (CurrentSet >= 2)
        {
            t1Scores[1] = t1s2Score;
            t2Scores[1] = t2s2Score;
            if (CurrentSet >= 3)
            {
                t1Scores[2] = t1s3Score;
                t2Scores[2] = t2s3Score;
            }
            else
            {
                t1Scores[2] = 0;
                t2Scores[2] = 0;
            }
        }
        else
        {
            t1Scores[1] = 0;
            t2Scores[1] = 0;
            t1Scores[2] = 0;
            t2Scores[2] = 0;
        }
        

        this.t1SetsWon = t1SetsWon;
        this.t2SetsWon = t2SetsWon;
    }
    
    public string print()
    {
        string output = GameNum.ToString() + ',' + teams[0] + ',' + teams[1] + ',' + GameOver.ToString() +','+ Winner + ',' + CurrentSet.ToString() +','+t1Scores[0].ToString() + ',' + t1Scores[1].ToString() + ',' + t1Scores[2].ToString() + ',' + t2Scores[0].ToString() + ',' + t2Scores[1].ToString() + ',' + t2Scores[2].ToString() + ','+t1SetsWon.ToString()+','+t2SetsWon.ToString();
        return output;
    }
}