using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScore
{
    internal class Match
    {
        private String player1, player2, player1_2, player2_2;
        private int matchLength, currentSet, currentScore1, currentScore2,  finalScore1, finalScore2;
        private int [] results1, results2;
        private bool gamePoint, matchPoint, wasGMP, setWon, gameWon;
        private bool service, isDouble, surrenderPlayer1, surrenderPlayer2;

        public Match()
        {
            player1 = "Player 1";
            player2 = "Player 2";
            player1_2 = "Name";
            Player2_2 = "Name";
            matchLength = 5;
            currentSet = currentScore1 = currentScore2 = finalScore1 = finalScore2 = 0;
            results1 = new int[matchLength];
            results2 = new int[matchLength];
            gamePoint = matchPoint = wasGMP = setWon = gameWon = surrenderPlayer1 = surrenderPlayer2 = false;
            isDouble = false;
            service = false;
        }

        public Match(String p1, String p2, String p1_2, String p2_2, int length, bool doubles, bool serviceP1)
        {
            player1 = p1;
            player2 = p2;
            player1_2 = p1_2;
            Player2_2 = p2_2;
            matchLength = length;
            currentSet = currentScore1 = currentScore2 = finalScore1 = finalScore2 = 0;
            results1 = new int[matchLength];
            results2 = new int[matchLength];
            gamePoint = matchPoint = wasGMP = setWon = gameWon = surrenderPlayer1 = surrenderPlayer2 = false;
            isDouble = doubles;
            service = serviceP1;
        }

        public bool addPoint1()
        {
            //If set not ended
            if (!setWon && !gameWon)
            {
                currentScore1++;
                //If !set just ended
                if (currentScore1 > 10 && currentScore1 - currentScore2 > 1)
                {
                    gamePoint = matchPoint = false;
                    setWon = wasGMP = true;
                    return true;
                }
                //Game  /  Match Point
                if ((currentScore1 > 9 && currentScore1 - currentScore2 > 0) || (currentScore2 > 9 && currentScore2 - currentScore1 > 0))
                {
                    bool advantage = gamePoint || matchPoint;
                    if (!advantage)
                    {
                        if (finalScore1 != matchLength / 2)
                            gamePoint = true;
                        else
                            matchPoint = true;
                    }
                    wasGMP = false;
                    return !advantage;
                }
                //Everything Else
                //Last point was 
                if (gamePoint || matchPoint)
                    wasGMP = true;
                gamePoint = matchPoint = false;
                return wasGMP;
            }
            return false;
        }

        public bool addPoint2()
        {
            //If set not ended
            if (!setWon && !gameWon)
            {
                currentScore2++;
                //If !set just ended
                if (currentScore2 > 10 && currentScore2 - currentScore1 > 1)
                {
                    gamePoint = matchPoint = false;
                    setWon = wasGMP = true;
                    return true;
                }
                //Game  /  Match Point
                if ((currentScore1 > 9 && currentScore1 - currentScore2 > 0) || (currentScore2 > 9 && currentScore2 - currentScore1 > 0))
                {
                    bool advantage = gamePoint || matchPoint;
                    if (!advantage)
                    {
                        if (finalScore2 != matchLength / 2)
                            gamePoint = true;
                        else
                            matchPoint = true;
                    }
                    wasGMP = false;
                    return !advantage;
                }
                //Everything Else
                //Last point was 
                if (gamePoint || matchPoint)
                    wasGMP = true;
                gamePoint = matchPoint = false;
                return wasGMP;
            }
            return false;
        }

        public bool subPoint1()
        {
            //If not 0 and set not ended
            if(currentScore1 != 0 && !setWon && !gameWon)
            {
                currentScore1--;
                //If just lost
                if (currentScore2 > 10 && currentScore2 - currentScore1 > 1)
                {
                    gamePoint = matchPoint = false;
                    wasGMP = setWon = true;
                    return true;
                }
                //Game  /  Match Point
                if ((currentScore1 > 9 && currentScore1 - currentScore2 > 0) || (currentScore2 > 9 && currentScore2 - currentScore1 > 0))
                {
                    bool advantage = gamePoint || matchPoint;
                    if (!advantage)
                    {
                        if (setWon)
                        {
                            if (finalScore1 != matchLength / 2)
                                gamePoint = true;
                            else
                                matchPoint = true;
                        }
                        else
                        {
                            if (finalScore2 != matchLength / 2)
                                gamePoint = true;
                            else
                                matchPoint = true;
                        }
                    }
                    wasGMP = false;
                    return !advantage;
                }
                //Everything Else
                //Last point was 
                if (gamePoint || matchPoint)
                    wasGMP = true;
                gamePoint = matchPoint = false;
                return wasGMP;
            }
            if(!(currentScore2 > 10 && currentScore2 - currentScore1 > 1) && currentScore1 != 0)
            {
                currentScore1--;
                gamePoint = matchPoint = true;
                wasGMP = setWon = false;
                return true;
            }
            return false;
        }

        public bool subPoint2()
        {
            //If not 0 and set not ended
            if (currentScore2 != 0 && !setWon && !gameWon)
            {
                currentScore2--;
                //If just lost
                if (currentScore1 > 10 && currentScore1 - currentScore2 > 1)
                {
                    gamePoint = matchPoint = false;
                    wasGMP = setWon = true;
                    return true;
                }
                //Game  /  Match Point
                if ((currentScore1 > 9 && currentScore1 - currentScore2 > 0) || (currentScore2 > 9 && currentScore2 - currentScore1 > 0))
                {
                    bool advantage = gamePoint || matchPoint;
                    if (!advantage)
                    {
                        if (setWon)
                        {
                            if (finalScore2 != matchLength / 2)
                                gamePoint = true;
                            else
                                matchPoint = true;
                        }
                        else
                        {
                            if (finalScore1 != matchLength / 2)
                                gamePoint = true;
                            else
                                matchPoint = true;
                        }
                    }
                    wasGMP = false;
                    return !advantage;
                }
                //Everything Else
                //Last point was 
                if (gamePoint || matchPoint)
                    wasGMP = true;
                gamePoint = matchPoint = false;
                return wasGMP;
            }
            if(!(currentScore1 > 10 && currentScore1 - currentScore2 > 1) && currentScore2 != 0)
            {
                currentScore2--;
                gamePoint = matchPoint = true;
                wasGMP = setWon = false;
                return true;
            }
            return false;
        }

        public bool nextGame()
        {
            if(finalScore1 <= matchLength / 2 && finalScore2 <= matchLength / 2)
            {
                currentSet++;
                service = !service;
                results1[currentSet-1] = currentScore1;
                results2[currentSet-1] = currentScore2;
                if (currentScore1 > currentScore2)
                    finalScore1++;
                else
                    finalScore2++;
                currentScore1 = 0;
                currentScore2 = 0;
                if ((finalScore1 > matchLength / 2 || finalScore2 > matchLength / 2))
                    gameWon = true;
            }
            setWon = false;
            wasGMP = false;
            return gameWon;
        }

        public bool surrender1()
        {
            if(!gameWon)
            {
                surrenderPlayer1 = true;
                gameWon = true;
                return true;
            }
            return false;
        }

        public bool surrender2()
        {
            if (!gameWon)
            {
                surrenderPlayer2 = true;
                gameWon = true;
                return true;
            }
            return false;
        }

        public bool isService()
        {
            if (CurrentScore1 >= 10 && CurrentScore2 >= 10)
            {
                if (!service)
                    return ((currentScore1 + currentScore2) % 2 < 1);
                return !((currentScore1 + currentScore2) % 2 < 1);
            }
            if (!service)
                return ((currentScore1 + currentScore2) % 4 < 2);
            return !((currentScore1 + currentScore2) % 4 < 2);
        }

        public string Player1 { get => player1; set => player1 = value; }
        public string Player2 { get => player2; set => player2 = value; }
        public string Player1_2 { get => player1_2; set => player1_2 = value; }
        public string Player2_2 { get => player2_2; set => player2_2 = value; }
        public int MatchLength { get => matchLength; set => matchLength = value; }
        public int CurrentSet { get => currentSet; set => currentSet = value; }
        public int CurrentScore1 { get => currentScore1; set => currentScore1 = value; }
        public int CurrentScore2 { get => currentScore2; set => currentScore2 = value; }
        public int FinalScore1 { get => finalScore1; set => finalScore1 = value; }
        public int FinalScore2 { get => finalScore2; set => finalScore2 = value; }
        public int[] Results1 { get => results1; set => results1 = value; }
        public int[] Results2 { get => results2; set => results2 = value; }
        public bool GamePoint { get => gamePoint; set => gamePoint = value; }
        public bool MatchPoint { get => matchPoint; set => matchPoint = value; }
        public bool WasGMP { get => wasGMP; set => wasGMP = value; }
        public bool SetWon { get => setWon; set => setWon = value; }
        public bool GameWon { get => gameWon; set => gameWon = value; }
        public bool Service { get => service; set => service = value; }
        public bool IsDouble { get => isDouble; set => isDouble = value; }
        public bool Surrender1 { get => surrenderPlayer1; set => surrenderPlayer1 = value; }
        public bool Surrender2 { get => surrenderPlayer2; set => surrenderPlayer2 = value; }
        

                
    }
}
