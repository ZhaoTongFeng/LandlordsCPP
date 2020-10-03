#pragma once
#include <vector>
#include <string>

//��Ϸ��NUM_SESSION���׶�
enum class GameSession
{
	PREPARE,
	START,
	CALL,
	PLAYING,
	FINISH,
	NUM_SESSION
};


//��Ϸ����ӿ�/����
//����������Ϸͨ�ýӿ�
class BaseGameMode
{
public:
	BaseGameMode();
	virtual ~BaseGameMode();

	//��������
	virtual void ProcessInput() = 0;

	//������Ϸ
	virtual void UpdateGame(float deltaTime) = 0;

	//���ͼ��
	virtual void GenerateOutput(std::string& str) = 0;


	//ѭ����ȡ��һ�����������
	void NextPlayer() { mCurPlayerIndex = (mCurPlayerIndex + 1) % 3; }
	//��ǰ���������
	class Player* GetCurPlayer() { return players[mCurPlayerIndex]; }


	//Setter/Getter
	void SetSession(GameSession sess) { mSession = sess; }
	const GameSession& GetSession() const { return mSession; }



protected:
	//������Ϸ
	virtual void ReStart() = 0;

	//��������
	virtual void HandCards() = 0;

	//��������
	virtual void Settle() = 0;

	//��Ϸ�׶�
	GameSession mSession = GameSession::PREPARE;


	//��ǰ�Ծ��е����
	std::vector<class Player*> players;
	//��ǰ���
	int mCurPlayerIndex = 0;

	//��ǰ���ƹ���
	class CardsBuffer* mCardsHandle = nullptr;





	//��������������
	int MAX_CARDS_COUNT = 0;

	//����ģʽ
	bool isDebug = true;

	///////////////
	//����

	//�����Ƿ����
	bool isFinish = false;

	//ʤ���Ķ���
	int winTeam = 0;

	//������
	std::string mSettleContent;


};

