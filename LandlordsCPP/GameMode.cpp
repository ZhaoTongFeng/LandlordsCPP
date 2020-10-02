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

	//ģ����Ҽ�����Ϸ
	Player* p = nullptr;

	for (int i = 0; i < 3; i++) {
		p = new Player("Player-" + std::to_string(i), 1000);
		players.emplace_back(p);
	}
	
	//ģ��ѡ����Ϸ����
	mCardsHandle = new CardsHandle(TOTAL_COUNT);

	//ģ�������Ϸ���ͣ���������
	Cards* c = nullptr;
	for (int i = 0; i < 3; i++) {
		p = players[i];
		c = new Cards(MAX_CARDS_COUNT);
		p->CreateCards(c);
	}
}


void GameMode::ReStart()
{
	//����
	for (int i = 0; i < 3; i++) {
		mDarkCards[i] = 0;
	}
	numCall = 0;
	mCallState = CallState::NO;
	mLandlordsIndex = -1;
	mCallArrCount = 0;
	mSession = GameSession::CALL;

	//����
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
	//����
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
		//������Ϸ
		if (key == 'r') {
			mSession = GameSession::START;
		}
		switch (mSession)
		{
		case GameSession::PREPARE:
			if (key == 's') {
				//��ʼ��Ϸ
				mSession = GameSession::START;
			}
			break;
		case GameSession::CALL:
			if (mCallState == CallState::NO) {
				if (key == 'q') {
					//��Ҫ
					inputPoint = 0;
				}
				else if (key == 'w') {
					//�е���
					mCallState = CallState::CALL;
					inputPoint = 1;
				}
			}
			else {
				if (key == 'q') {
					//��Ҫ
					inputPoint = 0;
				}
				else if (key == 'e') {
					//������
					mCallState = CallState::Rob;
					inputPoint = 2;
				}
			}

			break;
			
		case GameSession::PLAYING:
			if (mLastCards) {
				//����û����Ҫ��֮���ܲ�Ҫ
				if (key == 'n') {
					//��Ҫ
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
				//��ʼ����
				gate = true;
			}
			if (key == 'c') {
				//�����ѡ��
				mPreCards->MakeEmpty();
			}
			if (key == 'h') {
				//ȷ������
				Cards* ca = dynamic_cast<Cards*>(GetCurPlayer()->GetCardStack());
				if (ca->OutCards(mPreCards, mLastCards)) {
					delete mLastCards;
					mLastCards = mPreCards;
					mPreCards = new Cards(20);
					NextPlayer();
					mMissCount = 0;
				}
				//�������Ϊ�գ���ö���ʤ��
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
	//������Ϸ�׶ν��в�ͬ����
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
				//�����β���֮������ת��ֱ�ӿ�ʼ�ж�
				if (mCallArrCount == 0) {
					//���û���˽е����������¿�ʼ��Ϸ
					mSession = GameSession::START;
				}
				else if (mCallArrCount == 1) {
					mLandlordsIndex = mCallArr[0];
					isFinishCall = true;
				}
				else{
					//������������ѯ�ʽе�������
					mCurPlayerIndex = mLandlordsIndex;
				}
			}
			else if(numCall == 4){
				//����Ĵα�Ȼ����
				isFinishCall = true;
				if (mCallArrCount == 2) {
					//һ���˽е�����һ����ǹ
					if (inputPoint == 0) {
						//������������������
						mLandlordsIndex = mCallArr[1];
					}
					else if (inputPoint == 2) {
						mLandlordsIndex = mCallArr[0];
					}
				}
				else if (mCallArrCount == 3) {
					//һ���˽е�����������ǹ
					if (inputPoint == 0) {
						//���������¼�
						mLandlordsIndex = mCallArr[1];
					}
					else if (inputPoint == 2) {
						mLandlordsIndex = mCallArr[0];
					}
				}
			}

			if (isFinishCall) {
				//��ת���ƽ׶Σ����õ�ǰ���Ϊ�����������Ʒ��͵����������½�������
				mSession = GameSession::PLAYING;
				mCurPlayerIndex = mLandlordsIndex;
				for (int i = 0; i < 3; i++) {
					GetCurPlayer()->GetCardStack()->Push(mDarkCards[i]);
				}
				GetCurPlayer()->GetCardStack()->SortRank();
				//����
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
			//�����±꣬��ӵ���������
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
		str += "�ƶ�������" + std::to_string(cardsStackCount) + "\n";
		str += "�ƶ����棺";
		str += Util::GetPrintName(cardsStack, cardsStackCount);

		str += "���ŵ��ƣ�";
		str += Util::GetPrintName(mDarkCards, 3);



		for (int i = 0; i < 3; i++) {
			cardsStackCount = players[i]->GetCardStack()->GetSize();
			cardsStack = players[i]->GetCardStack()->GetBuf();
			str += "������ƣ�" + players[i]->GetName() + "\t";
			str += "���֣�" + std::to_string(players[i]->GetBalance()) + "\t";
			str += "����������" + std::to_string(cardsStackCount) + "\t";
			str += "���飺";
			if (players[i]->team == 1) {
				str += "����\n";
			}
			else if (players[i]->team == 2) {
				str += "ũ��\n";
			}
			else {
				str += "δ֪\n";
			}
			str += "���ƣ�";
			str += Util::GetPrintName(cardsStack, cardsStackCount);
		}
		str += "*****************************\n";
	}


	cardsStackCount = GetCurPlayer()->GetCardStack()->GetSize();
	cardsStack = GetCurPlayer()->GetCardStack()->GetBuf();


	if (mLandlordsIndex != -1) {
		str += "�������ƣ�" + players[mLandlordsIndex]->GetName() + "\n";
	}
	str += "��ǰ��ң�" + GetCurPlayer()->GetName() + "\n";

	str += "����������" + std::to_string(cardsStackCount) + "\n";
	str += "������ƣ�";
	str += Util::GetPrintName(cardsStack, cardsStackCount);




	switch (mSession)
	{
	case GameSession::PREPARE:
		str += "׼���׶�\n";
		str += "��S��ʼ";
		break;
	case GameSession::CALL:
		str += "���ƽ׶�\n";
		if (mCallState == CallState::NO) {
			str += "q\tw\n";
			str += "��Ҫ\t�е���\n";
		}
		else {
			str += "q\te\n";
			str += "��Ҫ\t������\n";
		}
		break;
	case GameSession::PLAYING:
		
		str += "���ƽ׶�\n\n";
		
		str += "��һ�γ��ƣ�";
		if (mLastCards) {
			str += Util::GetPrintName(mLastCards->GetBuf(), mLastCards->GetSize());
			str += "���ͣ�" + mLastCards->GetCardsModeName() + "\n";
		}
		else {
			str += "��\n";
		}
		str += "\n";

		str += "��Ҳ�����\n";
		if (!mLastCards) {
			str += "B��ѡ��Ҫ������\tH��ȷ������\tC���������\n";
		}
		else {
			str += "N����Ҫ\tB��ѡ��\tH������\tC�����\n";
		}
		
		str += "���ͣ�" + mPreCards->GetCardsModeName() + " ";
		str += "��ѡ��\n";
		str += Util::GetPrintName(mPreCards->GetBuf(), mPreCards->GetSize());




		break;
	case GameSession::FINISH:
		str += "����׶�\n";
		if (winTeam == 1) {
			str += "����ʤ����\n";
		}
		else {
			str += "ũ��ʤ����\n";
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
	//�Ƚ���������������
	for (i = 0; i < 3; i++) {
		players[i]->ResetAllCardStack();
	}
	//����ϴ��
	mCardsHandle->Shuffle();

	//�����
	//�������ŵ���
	for (i = 0; i < 3; i++) {
		mCardsHandle->Pop(mDarkCards[i]);
	}
	i = 0;
	//ʣ����ѭ���������
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
	//��������
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
		mSettleContent += "������" + player->GetName() + "+" + to_string(dou_coin) + "\n";
		for (int i = 0; i < 2; i++) {
			NextPlayer();
			player = GetCurPlayer();
			player->ChangeBalance(-1 * sig_coin);
			mSettleContent += "ũ��" + player->GetName() + "-" + to_string(sig_coin) + "\n";
		}
		
	}
	else if (winTeam == 2) {
		player->ChangeBalance(-1* dou_coin);
		mSettleContent += "������" + player->GetName() + "-" + to_string(dou_coin) + "\n";
		for (int i = 0; i < 2; i++) {
			NextPlayer();
			player = GetCurPlayer();
			player->ChangeBalance(sig_coin);
			mSettleContent += "ũ��" + player->GetName() + "+" + to_string(sig_coin) + "\n";
		}
	}
}

