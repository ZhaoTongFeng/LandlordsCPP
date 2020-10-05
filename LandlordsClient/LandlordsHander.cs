using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LandlordsCS
{
    //负责发牌和派送奖励
    public class LandlordsHander:CardsBuf
    {
		//倍数
		int rate = 1;

		//底分
		int baseBet = 100;

		//抽水
		int water = 20;
		

		public LandlordsHander(int size):base(size)
        {
			for (int i = 0; i < size; i++)
			{
				Push(i + 1);
			}
		}

		//返回出牌人下标
		public int HandCards(ref List<Player> players, ref CardsBuf darkCards)
		{
			//洗牌
			SetMax();
			Shuffle();

			//清空手牌
			foreach (Player item in players)
			{
				item.MakeCardsEmpty();
			}

			//发牌
			//先留三张底牌
			int i;
			for (i = 0; i < 3; i++)
			{
				Pop(ref darkCards);
			}

			i = 0;
			int n = 0;
			int firIndex = 0;
			//剩下牌循环发给玩家
			while (!IsEmpty())
			{
				Pop(ref n);
				players[i].GetCards().Push(n);
				//确定叫牌的人
				if (n != 53 && n != 54 && n % 54 == 10)
				{
					firIndex = i;
				}
				i = (i + 1) % 3;
			}

			//进行排序
			foreach (Player item in players)
			{
				item.GetCards().SortRank(false);
			}
			return firIndex;
		}

		public void Settle(ref List<Player> players, int winTeam, int landlordIndex)
		{
			int dou_coin, sig_coin;
			dou_coin = rate * baseBet - water;
			sig_coin = (int)(dou_coin / 2.0f);
			int i = 0;
            foreach (Player item in players)
            {
				if (item.mTeamID == winTeam)
				{
					if (i == landlordIndex)
					{
						item.mBalance += dou_coin;
					}
					else
					{
						item.mBalance += sig_coin;
					}
				}
				else
				{
					if (i == landlordIndex)
					{
						item.mBalance += -1*dou_coin;
					}
					else
					{
						item.mBalance += -1*sig_coin;
					}
				}
				i++;
			}
		}


	}
}
