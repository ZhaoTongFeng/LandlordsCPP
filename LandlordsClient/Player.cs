using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandlordsCS
{
    public class Player
    {
        public string mName;
        public int mBalance;
        public int mTeamID;
        public List<CardsBuf> cardsBufs;

        public Player(string name,int balance)
        {
            mName = name;
            mBalance = balance;
            mTeamID = -1;
            cardsBufs = new List<CardsBuf>();
        }
        

        public void AddCardsBuf(ref CardsBuf cardsBuf)
        {
            cardsBufs.Add(cardsBuf);
        }


        public CardsBuf GetCards(int i = 0) { return cardsBufs[i]; }


        public void MakeCardsEmpty()
        {
            foreach (CardsBuf item in cardsBufs)
            {
                item.MakeEmpty();
            }
        }

    }
}
