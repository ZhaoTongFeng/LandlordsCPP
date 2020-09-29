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

	//Setter/Getter
	void SetSession(GameSession sess) { mSession = sess; }
	const GameSession& GetSession() const { return mSession; }

protected:
	//��������
	virtual void HandCards() = 0;

	//��������
	virtual void Settle() = 0;

	//��Ϸ�׶�
	GameSession mSession;

	//��ǰ�Ծ��е����
	std::vector<class Player*> players;

	//��ǰ����±�
	int cur_player;

	//��ǰ���ƹ���
	class BaseCardsHandle* mCardsHandle;

	//���ŵ���
	int mDarkCards[3]{ 0 };

	//��������������
	int MAX_CARDS_COUNT;

	//����ģʽ
	bool isDebug;
};

