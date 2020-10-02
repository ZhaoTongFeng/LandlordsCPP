#include "GameMode.h"
#include <conio.h>
#include <iostream>

#include "Player.h"

#include "CardsHandle.h"
#include "Cards.h"

#include "Util.h"

#ifndef TOTAL_COUNT
#define TOTAL_COUNT 54
#endif // !TOTAL_COUNT


GameMode::GameMode()
{
	MAX_CARDS_COUNT = 20;
	mLastCards = nullptr;
	mPreCards = nullptr;

	//模拟玩家加入游戏
	Player* p = nullptr;

	for (int i = 0; i < 3; i++) {
		p = new Player("Player-" + std::to_string(i), 1000);
		players.emplace_back(p);
	}
	
	//模拟选择游戏类型
	mCardsHandle = new CardsHandle(TOTAL_COUNT);

	//模拟根据游戏类型，分配手牌
	Cards* c = nullptr;
	for (int i = 0; i < 3; i++) {
		p = players[i];
		c = new Cards(MAX_CARDS_COUNT);
		p->CreateCards(c);
	}
}


void GameMode::ReStart()
{
	//叫牌
	for (int i = 0; i < 3; i++) {
		mDarkCards[i] = 0;
	}
	numCall = 0;
	mCallState = CallState::NO;
	mLandlordsIndex = -1;
	mCallArrCount = 0;
	mSession = GameSession::CALL;

	//出牌
	mMissCount = 0;
	gate = false;
	if (mLastCards) {
		delete mLastCards;
		mLastCards = nullptr;
	}
	if (mPreCards) {
		delete mPreCards;
		mPreCards = new Cards(MAX_CARDS_COUNT);
	}
	else {
		mPreCards = new Cards(MAX_CARDS_COUNT);
	}
	//结算
	mSettleContent = "";
	rate = 1;
	baseBet = 100;
	water = 20;
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
		//重启游戏
		if (key == 'r') {
			mSession = GameSession::START;
		}
		switch (mSession)
		{
		case GameSession::PREPARE:
			if (key == 's') {
				//开始游戏
				mSession = GameSession::START;
			}
			break;
		case GameSession::CALL:
			if (mCallState == CallState::NO) {
				if (key == 'q') {
					//不要
					inputPoint = 0;
				}
				else if (key == 'w') {
					//叫地主
					mCallState = CallState::CALL;
					inputPoint = 1;
				}
			}
			else {
				if (key == 'q') {
					//不要
					inputPoint = 0;
				}
				else if (key == 'e') {
					//抢地主
					mCallState = CallState::Rob;
					inputPoint = 2;
				}
			}

			break;
			
		case GameSession::PLAYING:
			if (mLastCards) {
				//两次没有人要拍之后不能不要
				if (key == 'n') {
					//不要
					NextPlayer();
					mPreCards->MakeEmpty();
					mMissCount++;
					if (mMissCount == 2) {
						delete mLastCards;
						mLastCards = nullptr;
						mMissCount = 0;
					}
				}
			}

			if (key == 'b') {
				//开始出牌
				gate = true;
			}
			if (key == 'c') {
				//清除已选牌
				mPreCards->MakeEmpty();
			}
			if (key == 'h') {
				//确定出牌
				Cards* ca = dynamic_cast<Cards*>(GetCurPlayer()->GetCardStack());
				if (ca->OutCards(mPreCards, mLastCards)) {
					delete mLastCards;
					mLastCards = mPreCards;
					mPreCards = new Cards(20);
					NextPlayer();
					mMissCount = 0;
				}
				//如果手牌为空，则该队伍胜利
				if (ca->GetSize() == 0) {
					mSession = GameSession::FINISH;
				}
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
		ReStart();
		HandCards();
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
					mLandlordsIndex = mCallArr[0];
					isFinishCall = true;
				}
				else{
					//有人抢地主，询问叫地主的人
					mCurPlayerIndex = mLandlordsIndex;
				}
			}
			else if(numCall == 4){
				//最多四次必然结束
				isFinishCall = true;
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
			}

			if (isFinishCall) {
				//跳转出牌阶段，设置当前玩家为地主，将底牌发送到地主，重新进行排序
				mSession = GameSession::PLAYING;
				mCurPlayerIndex = mLandlordsIndex;
				for (int i = 0; i < 3; i++) {
					GetCurPlayer()->GetCardStack()->Push(mDarkCards[i]);
				}
				GetCurPlayer()->GetCardStack()->SortRank();
				//分组
				GetCurPlayer()->team = 1;
				NextPlayer();
				GetCurPlayer()->team = 2;
				NextPlayer();
				GetCurPlayer()->team = 2;
				NextPlayer();

			}
		}
		break;
	case GameSession::PLAYING:
		if (gate) {
			//输入下标，添加到待出牌组
			std::string mCardsBuffer;
			Cards* ca = dynamic_cast<Cards*>(GetCurPlayer()->GetCardStack());
			while (true) {
				std::cin >> mCardsBuffer;
				if (mCardsBuffer != "e") {
					mPreCards->Push(ca->GetBuf()[std::stoi(mCardsBuffer)]);
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
		if (!isFinish) {
			Settle();
			isFinish = true;
		}
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
		cardsStack = mCardsHandle->GetBuf();
		cardsStackCount = mCardsHandle->GetSize();
		str += "牌堆牌数：" + std::to_string(cardsStackCount) + "\n";
		str += "牌堆牌面：";
		str += Util::GetPrintName(cardsStack, cardsStackCount);

		str += "三张底牌：";
		str += Util::GetPrintName(mDarkCards, 3);



		for (int i = 0; i < 3; i++) {
			cardsStackCount = players[i]->GetCardStack()->GetSize();
			cardsStack = players[i]->GetCardStack()->GetBuf();
			str += "玩家名称：" + players[i]->GetName() + "\t";
			str += "积分：" + std::to_string(players[i]->GetBalance()) + "\t";
			str += "手牌数量：" + std::to_string(cardsStackCount) + "\t";
			str += "队伍：";
			if (players[i]->team == 1) {
				str += "地主\n";
			}
			else if (players[i]->team == 2) {
				str += "农民\n";
			}
			else {
				str += "未知\n";
			}
			str += "手牌：";
			str += Util::GetPrintName(cardsStack, cardsStackCount);
		}
		str += "*****************************\n";
	}


	cardsStackCount = GetCurPlayer()->GetCardStack()->GetSize();
	cardsStack = GetCurPlayer()->GetCardStack()->GetBuf();


	if (mLandlordsIndex != -1) {
		str += "地主名称：" + players[mLandlordsIndex]->GetName() + "\n";
	}
	str += "当前玩家：" + GetCurPlayer()->GetName() + "\n";

	str += "手牌数量：" + std::to_string(cardsStackCount) + "\n";
	str += "玩家手牌：";
	str += Util::GetPrintName(cardsStack, cardsStackCount);




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
		
		str += "出牌阶段\n\n";
		
		str += "上一次出牌：";
		if (mLastCards) {
			str += Util::GetPrintName(mLastCards->GetBuf(), mLastCards->GetSize());
			str += "牌型：" + mLastCards->GetCardsModeName() + "\n";
		}
		else {
			str += "无\n";
		}
		str += "\n";

		str += "玩家操作：\n";
		if (!mLastCards) {
			str += "B：选择要出的牌\tH：确定出牌\tC：清除出牌\n";
		}
		else {
			str += "N：不要\tB：选牌\tH：出牌\tC：清除\n";
		}
		
		str += "牌型：" + mPreCards->GetCardsModeName() + " ";
		str += "已选：\n";
		str += Util::GetPrintName(mPreCards->GetBuf(), mPreCards->GetSize());




		break;
	case GameSession::FINISH:
		str += "结算阶段\n";
		if (winTeam == 1) {
			str += "地主胜利！\n";
		}
		else {
			str += "农民胜利！\n";
		}
		//str += mSettleContent;
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
		mCardsHandle->Pop(mDarkCards[i]);
	}
	i = 0;
	//剩下牌循环发给玩家
	while (!mCardsHandle->IsEmpty())
	{
		int n;
		mCardsHandle->Pop(n);
		players[i]->GetCardStack()->Push(n);
		if (n != 53 && n != 54 && n % 54 == 10) {
			mCurPlayerIndex = i;
		}
		i = (i + 1) % 3;
	}
	//进行排序
	for (i = 0; i < 3; i++) {
		players[i]->GetCardStack()->SortRank();
	}
}

void GameMode::Settle()
{
	mCurPlayerIndex = mLandlordsIndex;
	Player* player = GetCurPlayer();
	
	int dou_coin, sig_coin;
	dou_coin = rate * baseBet - water;
	sig_coin = static_cast<int>(static_cast<float>(dou_coin) / 2.0f);
	if (winTeam == 1) {
		player->ChangeBalance(dou_coin);
		mSettleContent += "地主：" + player->GetName() + "+" + to_string(dou_coin) + "\n";
		for (int i = 0; i < 2; i++) {
			NextPlayer();
			player = GetCurPlayer();
			player->ChangeBalance(-1 * sig_coin);
			mSettleContent += "农民：" + player->GetName() + "-" + to_string(sig_coin) + "\n";
		}
		
	}
	else if (winTeam == 2) {
		player->ChangeBalance(-1* dou_coin);
		mSettleContent += "地主：" + player->GetName() + "-" + to_string(dou_coin) + "\n";
		for (int i = 0; i < 2; i++) {
			NextPlayer();
			player = GetCurPlayer();
			player->ChangeBalance(sig_coin);
			mSettleContent += "农民：" + player->GetName() + "+" + to_string(sig_coin) + "\n";
		}
	}
}

