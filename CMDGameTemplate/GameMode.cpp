#include "GameMode.h"
#include <conio.h>
#include "Player.h"
#include "CardsHandle.h"
#include "Cards.h"

GameMode::GameMode()
{
	MAX_CARDS_COUNT = 20;
	isDebug = true;

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
	//������Ϸ�׶ν��в�ͬ����
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
		str += "��S��ʼ";
		break;
	case GameSession::CALL:
		if (isDebug) {
			str += "*****************************\n";
			int cardsStackCount = mCardsHandle->GetCardsCount();
			str += "�ƶ�������" + std::to_string(cardsStackCount) + "\n";
			str += "�ƶ����棺";
			const int* cardsStack = mCardsHandle->GetCards();
			for (int i = 0; i < cardsStackCount; i++) {
				str += BaseCards::GetCardName(cardsStack[i]) + " ";
			}
			str += "\n";

			str += "���ŵ��ƣ�";
			for (int i = 0; i < 3; i++) {
				str += BaseCards::GetCardName(mDarkCards[i])+" ";
			}
			str += "\n";


			for (int i = 0; i < 3; i++) {
				cardsStackCount = players[i]->GetCards()[0]->GetCardsCount();
				cardsStack = players[i]->GetCards()[0]->GetCards();
				str += "������ƣ�" + players[i]->GetName() + "\n";
				str += "����������" + std::to_string(cardsStackCount) + "\n";
				str += "������ƣ�";

				for (int i = 0; i < cardsStackCount; i++) {
					str += BaseCards::GetCardName(cardsStack[i]) + " ";
				}
				str += "\n";
			}
			str += "*****************************\n";
		}
		str += "���ƽ׶�\n";
		str += "��ǰ��ң�" + players[cur_player]->GetName()+"\n";
		str += "1\t2\t3\n";
		str += "��Ҫ\t�е���\t������\n";
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
	//�Ƚ���������������
	for (i = 0; i < 3; i++) {
		players[i]->ClearAllCardStack();
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
		mCardsHandle->HandCard(players[i++]->GetCards()[0]);
		i %= 3;
	}
}

void GameMode::Settle()
{
	//���ݲ�ͬ����Ӯ������н���
}
