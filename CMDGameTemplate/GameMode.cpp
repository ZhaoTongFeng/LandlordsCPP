#include "GameMode.h"
#include <conio.h>
#include "Player.h"
#include "CardsHandle.h"
#include "Cards.h"

GameMode::GameMode()
{
	MAX_CARDS_COUNT = 20;
	isDebug = true;

	//模拟玩家加入游戏
	Player* p = nullptr;
	for (int i = 0; i < 3; i++) {
		p = new Player("Player-" + std::to_string(i), 1000);
		players.emplace_back(p);
	}
	
	//模拟选择游戏类型
	mCardsHandle = new CardsHandle(54);

	//模拟根据游戏类型，分配手牌
	Cards* c = nullptr;
	for (int i = 0; i < 3; i++) {
		p = players[i];
		c = new Cards(MAX_CARDS_COUNT);
		p->CreateCards(c);
	}
}


GameMode::~GameMode()
{
	for (int i = 0; i < 3; i++) {
		delete players[i];
	}
}


void GameMode::ProcessInput()
{
	if (_kbhit()) {
		int key = _getch();
		switch (mSession)
		{
		case GameSession::PREPARE:
			if (key == 's') {
				mSession = GameSession::START;
			}
			break;
		case GameSession::START:
			break;
		case GameSession::CALL:
			break;
		case GameSession::PLAYING:
			break;
		case GameSession::FINISH:
			break;
		default:
			break;
		}
	}
}

void GameMode::UpdateGame(float deltaTime)
{
	//根据游戏阶段进行不同操作
	switch (mSession)
	{
	case GameSession::START:
		HandCards();
		mSession = GameSession::CALL;
		break;
	case GameSession::CALL:
		break;
	case GameSession::PLAYING:
		break;
	case GameSession::FINISH:
		break;
	default:
		break;
	}
}

void GameMode::GenerateOutput(std::string& str)
{
	switch (mSession)
	{
	case GameSession::PREPARE:
		str += "按S开始";
		break;
	case GameSession::CALL:
		if (isDebug) {
			str += "*****************************\n";
			int cardsStackCount = mCardsHandle->GetCardsCount();
			str += "牌堆牌数：" + std::to_string(cardsStackCount) + "\n";
			str += "牌堆牌面：";
			const int* cardsStack = mCardsHandle->GetCards();
			for (int i = 0; i < cardsStackCount; i++) {
				str += BaseCards::GetCardName(cardsStack[i]) + " ";
			}
			str += "\n";

			str += "三张底牌：";
			for (int i = 0; i < 3; i++) {
				str += BaseCards::GetCardName(mDarkCards[i])+" ";
			}
			str += "\n";


			for (int i = 0; i < 3; i++) {
				cardsStackCount = players[i]->GetCards()[0]->GetCardsCount();
				cardsStack = players[i]->GetCards()[0]->GetCards();
				str += "玩家名称：" + players[i]->GetName() + "\n";
				str += "手牌数量：" + std::to_string(cardsStackCount) + "\n";
				str += "玩家手牌：";

				for (int i = 0; i < cardsStackCount; i++) {
					str += BaseCards::GetCardName(cardsStack[i]) + " ";
				}
				str += "\n";
			}
			str += "*****************************\n";
		}
		str += "叫牌阶段\n";
		str += "当前玩家：" + players[cur_player]->GetName()+"\n";
		str += "1\t2\t3\n";
		str += "不要\t叫地主\t抢地主\n";
		break;
	case GameSession::PLAYING:
		break;
	case GameSession::FINISH:
		break;
	default:
		break;
	}
}

void GameMode::HandCards()
{
	int i;
	//先将所有玩家手牌清空
	for (i = 0; i < 3; i++) {
		players[i]->ClearAllCardStack();
	}
	//接着洗牌
	mCardsHandle->Shuffle();

	//最后发牌
	//先留三张底牌
	for (i = 0; i < 3; i++) {
		mCardsHandle->HandCard(mDarkCards[i]);
	}
	i = 0;
	//剩下牌循环发给玩家
	while (!mCardsHandle->isEmpty())
	{
		mCardsHandle->HandCard(players[i++]->GetCards()[0]);
		i %= 3;
	}
}

void GameMode::Settle()
{
	//根据不同的输赢结果进行结算
}
