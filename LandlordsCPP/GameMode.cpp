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



	//ģ����Ҽ�����Ϸ
	Player* p = nullptr;
	for (int i = 0; i < 3; i++) {
		p = new Player("Player-" + std::to_string(i), 1000);
		players.emplace_back(p);
	}
	
	//ģ��ѡ����Ϸ����
	mCardsHandle = new CardsHandle(54);

	//ģ�������Ϸ���ͣ���������
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
			//��ʼ����
			if (key == 'b') {
				gate = true;
			}
			//���ѡ��
			if (key == 'c') {
				mPreOutCardsCount = 0;
			}
			//������
			if (key == 'n') {
				NextPlayer();
				mPreOutCardsCount = 0;
			}
			//ȷ������
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
	//������Ϸ�׶ν��в�ͬ����
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
				//�����β���֮������ת��ֱ�ӿ�ʼ�ж�
				if (mCallArrCount == 0) {
					//���û���˽е����������¿�ʼ��Ϸ
					mSession = GameSession::START;
				}
				else if (mCallArrCount == 1) {
					std::cout << "ֻ��һ���˽е�����û����ǹ";
					mLandlordsIndex = mCallArr[0];
					isFinishCall = true;
				}
				else{
					//һ���˽е�����������ǹ
					//һ���˽е�����һ����ǹ
					//ѯ�ʵ�ǰ�ĵ���
					mCurPlayerIndex = mLandlordsIndex;
				}
			}
			else if(numCall == 4){
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
		str += "�ƶ�������" + std::to_string(cardsStackCount) + "\n";
		str += "�ƶ����棺";
		cardsStack = mCardsHandle->GetCards();
		for (int i = 0; i < cardsStackCount; i++) {
			str += BaseCards::GetCardName(cardsStack[i]) + " ";
		}
		str += "\n";

		str += "���ŵ��ƣ�";
		for (int i = 0; i < 3; i++) {
			str += BaseCards::GetCardName(mDarkCards[i]) + " ";
		}
		str += "\n";


		for (int i = 0; i < 3; i++) {
			cardsStackCount = players[i]->GetCardStack()->GetCardsCount();
			cardsStack = players[i]->GetCardStack()->GetCards();
			str += "������ƣ�" + players[i]->GetName() + "\n";
			str += "����������" + std::to_string(cardsStackCount) + "\n";
			str += "��ҽ��ƣ�" + std::to_string(players[i]->point) + "\n";
			str += "������ƣ�";

			for (int i = 0; i < cardsStackCount; i++) {
				str += BaseCards::GetCardName(cardsStack[i]) + " ";
			}
			str += "\n";
		}

		str += "������Ƶ����" + std::to_string(mCallArrCount) + "\n";

		str += "*****************************\n";
	}

	str += "��ǰ��ң�" + GetCurPlayer()->GetName() + "\n";
	cardsStackCount = GetCurPlayer()->GetCardStack()->GetCardsCount();
	cardsStack = GetCurPlayer()->GetCardStack()->GetCards();

	str += "����������" + std::to_string(cardsStackCount) + "\n";
	str += "������ƣ�";
	for (int i = 0; i < cardsStackCount; i++) {
		str += BaseCards::GetCardName(cardsStack[i]) + " ";
	}
	str += "\n";





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
		str += "���ƽ׶�\n";
		str += "�������ƣ�" + players[mLandlordsIndex]->GetName()+"\n";
		str += "n����Ҫ\tb��ѡ��Ҫ������\th��ȷ������\tc���������\t\n";
		str += "��ѡ��\n";
		for (int i = 0; i < mPreOutCardsCount; i++) {
			str += std::to_string(mPreOutCards[i])+"\t"+BaseCards::GetCardName(cardsStack[mPreOutCards[i]]) + "\n";
		}
		str += "\n";
		str += "������ƣ�\n";
		for (int i = 0; i < cardsStackCount; i++) {
			str += std::to_string(i) +"\t"+ BaseCards::GetCardName(cardsStack[i]) + "\n";
		}
		str += "\n";


		break;
	case GameSession::FINISH:
		str += "����׶�\n";
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
		mCardsHandle->HandCard(mDarkCards[i]);
	}
	i = 0;
	//ʣ����ѭ���������
	while (!mCardsHandle->isEmpty())
	{
		int n = mCardsHandle->HandCard(players[i]->GetCardStack());
		if (n != 53 && n != 54 && n % 54 == 10) {
			mCurPlayerIndex = i;
		}
		i = (i + 1) % 3;
	}
	//��������
	for (i = 0; i < 3; i++) {
		players[i]->GetCardStack()->SortCards();
	}
}

void GameMode::Settle()
{
	//���ݲ�ͬ����Ӯ������н���
}
