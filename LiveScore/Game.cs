using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScore
{
    internal class Game
    {
		private String name1;
		private String name2;
		private String auxName1;
		private String auxName2;
		private int nSets;
		private int setIndex;
		private int currentPoints1;
		private int currentPoints2;
		private int[] result1;
		private int[] result2;
		private int final1;
		private int final2;
		private bool setPoint;
		private bool gamePoint;
		private bool surrender1;
		private bool surrender2;
		private bool service;
		private bool won, gameWon;

		public Game()
		{
			name1 = "Player1";
			name2 = "Player2";
			auxName1 = auxName2 = "Name";
			nSets = 5;
			setIndex = 0;
			result1 = new int[nSets];
			result2 = new int[nSets];
			final1 = 0;
			final2 = 0;
			currentPoints1 = 0;
			currentPoints2 = 0;
			setPoint = gamePoint = false;
			surrender1 = surrender2 = false;
			service = true;
			gameWon = won = false;
		}

		public Game(int number, String p1, String p2, String p3, String p4)
		{
			name1 = p1;
			name2 = p2;
			auxName1 = p3;
			auxName2 = p4;
			nSets = number;
			setIndex = 0;
			result1 = new int[nSets];
			result2 = new int[nSets];
			final1 = 0;
			final2 = 0;
			currentPoints1 = 0;
			currentPoints2 = 0;
			setPoint = gamePoint = false;
			surrender1 = surrender2 = false;
			service = true;
			gameWon = won = false;
		}

        public string Name1 { get => name1; set => name1 = value; }
        public string Name2 { get => name2; set => name2 = value; }
        public int[] Result1 { get => result1; set => result1 = value; }
        public int[] Result2 { get => result2; set => result2 = value; }
        public int Final1 { get => final1; set => final1 = value; }
        public int Final2 { get => final2; set => final2 = value; }
        public bool SetPoint { get => setPoint; set => setPoint = value; }
        public bool GamePoint { get => gamePoint; set => gamePoint = value; }
        public int CurrentPoints1 { get => currentPoints1; }
        public int CurrentPoints2 { get => currentPoints2; }
        public int NSets { get => nSets; set => nSets = value; }
        public int SetIndex { get => setIndex; set => setIndex = value; }
        public bool Surrender1 { get => surrender1; set => surrender1 = value; }
        public bool Surrender2 { get => surrender2; set => surrender2 = value; }
        public bool Service { get => service; set => service = value; }
        public bool Won { get => won; set => won = value; }
        public string AuxName1 { get => auxName1; set => auxName1 = value; }
        public string AuxName2 { get => auxName2; set => auxName2 = value; }
        public bool GameWon { get => gameWon; set => gameWon = value; }

        public void nextSet()
        {
			if (setIndex < nSets)
			{
				service = !service;
				if (final1 <= nSets / 2 && final2 <= nSets / 2)
				{
					result1[setIndex] = currentPoints1;
					result2[setIndex] = currentPoints2;

					if (currentPoints1 > currentPoints2)
						final1++;
					else
						final2++;
					currentPoints1 = currentPoints2 = 0;
					if (final1 <= nSets / 2 && final2 <= nSets / 2)
						setIndex++;
					else
						gameWon = true;
				}
				else
					gameWon = true;
			}
			else
				gameWon = true;
        }

		public void reset(int number, String p1, String p2, String p3, String p4)
		{
			name1 = p1;
			name2 = p2;
			auxName1 = p3;
			auxName2 = p4;
			nSets = number;
			setIndex = 0;
			result1 = new int[nSets];
			result2 = new int[nSets];
			final1 = 0;
			final2 = 0;
			currentPoints1 = 0;
			currentPoints2 = 0;
			setPoint = gamePoint = false;
		}

		public bool addPoint1()
        {
			//Set Already Ended
			if(!((currentPoints1 > 10 && currentPoints1-currentPoints2 > 1) || (currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1)))
            {
				currentPoints1++;

				//Set Just Ended
				if((currentPoints1 > 10 && currentPoints1-currentPoints2 > 1) || (currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1))
                {
					setPoint = gamePoint = false;
					won = true;
					return true;
                }
				//Set Point
				if (currentPoints1 > 9 && currentPoints1 - currentPoints2 > 0)
				{
					if (final1 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				else if (currentPoints2 > 9 && currentPoints2 - currentPoints1 > 0)
				{
					if (final2 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				//advantace 
				else if (currentPoints1 > 9 && currentPoints1 == currentPoints2)
                {
					setPoint = gamePoint = false;
					won = true;
                }
				//Nothing
				else
				{
					setPoint = false;
					gamePoint = false;
					won = false;
				}
				return false;
            }
			won = true;
			return true;
        }

		public bool addPoint2()
		{
			//Set Already Ended
			if (!((currentPoints1 > 10 && currentPoints1 - currentPoints2 > 1) || (currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1)))
			{
				currentPoints2++;

				//Set Just Ended
				if ((currentPoints1 > 10 && currentPoints1 - currentPoints2 > 1) || (currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1))
				{
					setPoint = gamePoint = false;
					won = true;
					return true;
				}
				//Set Point
				if (currentPoints1 > 9 && currentPoints1 - currentPoints2 > 0)
				{
					if (final1 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				else if (currentPoints2 > 9 && currentPoints2 - currentPoints1 > 0)
				{
					if (final2 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				//advantace 
				else if (currentPoints1 > 9 && currentPoints1 == currentPoints2)
				{
					setPoint = gamePoint = false;
					won = true;
				}
				//Nothing
				else
				{
					setPoint = false;
					gamePoint = false;
					won = false;
				}
				return false;
			}
			won = true;
			return true;
		}

		public bool subPoint1()
		{
			//Not 0 Not Lost
			if ((currentPoints1 > 0) && !(currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1))
			{
				currentPoints1--;

				//Just Lost
				if (currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1)
                {
					setPoint = gamePoint = false;
					won = true;
					return true;
                }

				//Set Point
				if (currentPoints1 > 9 && currentPoints1 - currentPoints2 > 0)
				{
					if (final1 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				else if (currentPoints2 > 9 && currentPoints2 - currentPoints1 > 0)
				{
					if (final2 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				//advantace 
				else if (currentPoints1 > 9 && currentPoints1 == currentPoints2)
				{
					setPoint = gamePoint = false;
					won = true;
				}
				else if(currentPoints1 > 8)
                {
					setPoint = gamePoint = false;
					won = true;
				}
				//Nothing
				else
				{
					setPoint = false;
					gamePoint = false;
					won = false;
				}
				return false;
			}
			if (currentPoints2 > 10 && currentPoints2 - currentPoints1 > 1)
            {
				won = true;
				return true;
            }
			won = false;
			return false;
		}

		public bool subPoint2()
		{
			//Not 0 Not Lost
			if ((currentPoints2 > 0) && !(currentPoints1 > 10 && currentPoints1 - currentPoints2 > 1))
			{
				currentPoints2--;

				//Just Lost
				if (currentPoints1 > 10 && currentPoints1 - currentPoints2 > 1)
                {
					setPoint = gamePoint = false;
					won = true;
					return true;
                }

				//Set Point
				if (currentPoints1 > 9 && currentPoints1 - currentPoints2 > 0)
				{
					if (final1 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				else if (currentPoints2 > 9 && currentPoints2 - currentPoints1 > 0)
				{
					if (final2 != nSets / 2)
						setPoint = true;
					else
						gamePoint = true;
					won = false;
				}
				//advantace 
				else if (currentPoints1 > 9 && currentPoints1 == currentPoints2)
				{
					setPoint = gamePoint = false;
					won = true;
				}
				else if (currentPoints2 > 8)
				{
					setPoint = gamePoint = false;
					won = true;
				}
				//Nothing
				else
				{
					setPoint = false;
					gamePoint = false;
					won = false;
				}
				return false;
			}
			if (currentPoints1 > 10 && currentPoints1 - currentPoints2 > 1)
            {
				won = true;
				return true;
            }
			won = false;
			return false;
		}

		public bool isService()
        {
            if (service)
            {
				if ((currentPoints1 + currentPoints2) % 4 < 2)
					return true;
				return false;
            }
            else
            {
				if ((currentPoints1 + currentPoints2) % 4 < 2)
					return false;
				return true;
			}
			
        }

	}
}
