#include "GameMode.h"
#include <conio.h>
#include "Player.h"
#include "CardsHandle.h"
#include "Cards.h"
#include <iostream>

GameMode::GameMode()
{
	MAX_CARDS_COUNT = 20;
	isDebug = true;
	mCurPlayerIndex = 0;



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
	inputPoint = -1;
	if (_kbhit()) {
		int key = _getch();
		switch (mSession)
		{
		case GameSession::PREPARE:
			if (key == 's') {
				mSession = GameSession::START;
			}
			break;
		case GameSession::CALL:
			if (mCallState == CallState::NO) {
				if (key == 'q') {
					inputPoint = 0;
				}
				else if (key == 'w') {
					mCallState = CallState::CALL;
					inputPoint = 1;
				}
			}
			else {
				if (key == 'q') {
					inputPoint = 0;
				}
				else if (key == 'e') {
					mCallState = CallState::Rob;
					inputPoint = 2;
				}
			}

			break;
			
		case GameSession::PLAYING:
			//开始出牌
			if (key == 'b') {
				gate = true;
			}
			//清除选择
			if (key == 'c') {
				mPreOutCardsCount = 0;
			}
			//不出牌
			if (key == 'n') {
				NextPlayer();
				mPreOutCardsCount = 0;
			}
			//确定出牌
			if (key == 'h') {
				Cards* ca = dynamic_cast<Cards*>(GetCurPlayer()->GetCardStack());
				ca->OutCards(mPreOutCards, mPreOutCardsCount);
				NextPlayer();
				mPreOutCardsCount = 0;
			}



			
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
		numCall = 0;
		mCallState = CallState::NO;
		mLandlordsIndex = -1;
		mCallArrCount = 0;
		mSession = GameSession::CALL;
		break;
	case GameSession::CALL:
		if (inputPoint != -1) {
			numCall++;

			bool isFinishCall = false;

			if (numCall <= 2) {
				if (inputPoint == 1) {
					mLandlordsIndex = mCurPlayerIndex;
				}
				if (inputPoint != 0) {
					mCallArr[mCallArrCount++] = mCurPlayerIndex;
				}
				NextPlayer();
			}
			else if (numCall == 3) {
				
				if (inputPoint == 1) {
					mLandlordsIndex = mCurPlayerIndex;
				}
				if (inputPoint != 0) {
					mCallArr[mCallArrCount++] = mCurPlayerIndex;
				}
				//第三次操作之后不用跳转，直接开始判断
				if (mCallArrCount == 0) {
					//如果没有人叫地主，则重新开始游戏
					mSession = GameSession::START;
				}
				else if (mCallArrCount == 1) {
					std::cout << "只有一个人叫地主，没有人枪";
					mLandlordsIndex = mCallArr[0];
					isFinishCall = true;
				}
				else{
					//一个人叫地主，两个人枪
					//一个人叫地主，一个人枪
					//询问当前的地主
					mCurPlayerIndex = mLandlordsIndex;
				}
			}
			else if(numCall == 4){
				if (mCallArrCount == 2) {
					//一个人叫地主，一个人枪
					if (inputPoint == 0) {
						//放弃，归抢地主的人
						mLandlordsIndex = mCallArr[1];
					}
					else if (inputPoint == 2) {
						mLandlordsIndex = mCallArr[0];
					}
				}
				else if (mCallArrCount == 3) {
					//一个人叫地主，两个人枪
					if (inputPoint == 0) {
						//放弃，归下家
						mLandlordsIndex = mCallArr[1];
					}
					else if (inputPoint == 2) {
						mLandlordsIndex = mCallArr[0];
					}
				}
				isFinishCall = true;
			}

			if (isFinishCall) {
				mSession = GameSession::PLAYING;
				mCurPlayerIndex = mLandlordsIndex;
			}
			
		}
		break;
	case GameSession::PLAYING:
		if (gate) {
			while (true)
			{
				std::cin >> mCardsBuffer;
				if (mCardsBuffer != "e") {
					mPreOutCards[mPreOutCardsCount++] = std::stoi(mCardsBuffer);
				}
				else {
					break;
				}
				mCardsBuffer = "";
			}
			gate = false;
		}
		break;
	case GameSession::FINISH:
		break;
	default:
		break;
	}
}

void GameMode::GenerateOutput(std::string& str)
{
	int cardsStackCount;
	const int* cardsStack;
	if (isDebug) {
		str += "*****************************\n";
		cardsStackCount = mCardsHandle->GetCardsCount();
		str += "牌堆牌数：" + std::to_string(cardsStackCount) + "\n";
		str += "牌堆牌面：";
		cardsStack = mCardsHandle->GetCards();
		for (int i = 0; i < cardsStackCount; i++) {
			str += BaseCards::GetCardName(cardsStack[i]) + " ";
		}
		str += "\n";

		str += "三张底牌：";
		for (int i = 0; i < 3; i++) {
			str += BaseCards::GetCardName(mDarkCards[i]) + " ";
		}
		str += "\n";


		for (int i = 0; i < 3; i++) {
			cardsStackCount = players[i]->GetCardStack()->GetCardsCount();
			cardsStack = players[i]->GetCardStack()->GetCards();
			str += "玩家名称：" + players[i]->GetName() + "\n";
			str += "手牌数量：" + std::to_string(cardsStackCount) + "\n";
			str += "玩家叫牌：" + std::to_string(players[i]->point) + "\n";
			str += "玩家手牌：";

			for (int i = 0; i < cardsStackCount; i++) {
				str += BaseCards::GetCardName(cardsStack[i]) + " ";
			}
			str += "\n";
		}

		str += "参与叫牌的玩家" + std::to_string(mCallArrCount) + "\n";

		str += "*****************************\n";
	}

	str += "当前玩家：" + GetCurPlayer()->GetName() + "\n";
	cardsStackCount = GetCurPlayer()->GetCardStack()->GetCardsCount();
	cardsStack = GetCurPlayer()->GetCardStack()->GetCards();

	str += "手牌数量：" + std::to_string(cardsStackCount) + "\n";
	str += "玩家手牌：";
	for (int i = 0; i < cardsStackCount; i++) {
		str += BaseCards::GetCardName(cardsStack[i]) + " ";
	}
	str += "\n";





	switch (mSession)
	{
	case GameSession::PREPARE:
		str += "准备阶段\n";
		str += "按S开始";
		break;
	case GameSession::CALL:
		str += "叫牌阶段\n";
		if (mCallState == CallState::NO) {
			str += "q\tw\n";
			str += "不要\t叫地主\n";
		}
		else {
			str += "q\te\n";
			str += "不要\t抢地主\n";
		}
		break;
	case GameSession::PLAYING:
		str += "出牌阶段\n";
		str += "地主名称：" + players[mLandlordsIndex]->GetName()+"\n";
		str += "n：不要\tb：选择要出的牌\th：确定出牌\tc：清除出牌\t\n";
		str += "已选：\n";
		for (int i = 0; i < mPreOutCardsCount; i++) {
			str += std::to_string(mPreOutCards[i])+"\t"+BaseCards::GetCardName(cardsStack[mPreOutCards[i]]) + "\n";
		}
		str += "\n";
		str += "玩家手牌：\n";
		for (int i = 0; i < cardsStackCount; i++) {
			str += std::to_string(i) +"\t"+ BaseCards::GetCardName(cardsStack[i]) + "\n";
		}
		str += "\n";


		break;
	case GameSession::FINISH:
		str += "结算阶段\n";
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
		players[i]->ResetAllCardStack();
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
		int n = mCardsHandle->HandCard(players[i]->GetCardStack());
		if (n != 53 && n != 54 && n % 54 == 10) {
			mCurPlayerIndex = i;
		}
		i = (i + 1) % 3;
	}
	//进行排序
	for (i = 0; i < 3; i++) {
		players[i]->GetCardStack()->SortCards();
	}
}

void GameMode::Settle()
{
	//根据不同的输赢结果进行结算
}
