using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScore
{
    internal class TeamMatch
    {
        private String team1, team2;
        private int matchLength, currentMatch, final1, final2;
        private Match[] matchResults;
        bool matchWon, surrenderTeam1, surrenderTeam2;

        public TeamMatch()
        {
            team1 = "Team 1";
            team2 = "Team 2";
            matchLength = 9;
            currentMatch = final1 = final2 = 0;
            matchResults = new Match[matchLength];
            matchWon = surrenderTeam1 = surrenderTeam2 = false;
        }

        public TeamMatch(String t1, String t2, int length)
        {
            team1 = t1;
            team2 = t2;
            matchLength = length;
            currentMatch = final1 = final2 = 0;
            matchResults = new Match[matchLength];
            matchWon = surrenderTeam1 = surrenderTeam2 = false;
        }

        public bool nextMatch(Match m)
        {
            if(!matchWon)
            {
                matchResults[currentMatch] = m;
                currentMatch++;
                if (m.Surrender1)
                    final2++;
                else if (m.Surrender1)
                    final2++;
                else if (m.FinalScore1 > m.FinalScore2)
                    final1++;
                else
                    final2++;
                return false;
            }
            return true;
        }

        private bool surrender1()
        {
            if (!matchWon)
            {
                surrenderTeam1 = true;
                matchWon = true;
            }
            return false;
        }

        private bool surrender2()
        {
            if (!matchWon)
            {
                surrenderTeam2 = true;
                matchWon = true;
            }
            return false;
        }

        public string Team1 { get => team1; set => team1 = value; }
        public string Team2 { get => team2; set => team2 = value; }
        public int MatchLength { get => matchLength; set => matchLength = value; }
        public int CurrentMatch { get => currentMatch; set => currentMatch = value; }
        public int Final1 { get => final1; set => final1 = value; }
        public int Final2 { get => final2; set => final2 = value; }
        public bool MatchWon { get => matchWon; set => matchWon = value; }
        public bool SurrenderTeam1 { get => surrenderTeam1; set => surrenderTeam1 = value; }
        public bool SurrenderTeam2 { get => surrenderTeam2; set => surrenderTeam2 = value; }
        internal Match[] MatchResults { get => matchResults; set => matchResults = value; }
    }
}
