#pragma once
#include <vector>
#include <string>

//游戏的NUM_SESSION个阶段
enum class GameSession
{
	PREPARE,
	START,
	CALL,
	PLAYING,
	FINISH,
	NUM_SESSION
};


//游戏规则接口/基类
//所有棋牌游戏通用接口
class BaseGameMode
{
public:
	BaseGameMode();
	virtual ~BaseGameMode();

	//按键输入
	virtual void ProcessInput() = 0;

	//更新游戏
	virtual void UpdateGame(float deltaTime) = 0;

	//输出图像
	virtual void GenerateOutput(std::string& str) = 0;

	//Setter/Getter
	void SetSession(GameSession sess) { mSession = sess; }
	const GameSession& GetSession() const { return mSession; }

protected:
	//发牌序列
	virtual void HandCards() = 0;

	//结算序列
	virtual void Settle() = 0;

	//游戏阶段
	GameSession mSession;

	//当前对局中的玩家
	std::vector<class Player*> players;

	//当前玩家下标
	int cur_player;

	//当前发牌规则
	class BaseCardsHandle* mCardsHandle;

	//三张底牌
	int mDarkCards[3]{ 0 };

	//玩家最大手牌数量
	int MAX_CARDS_COUNT;

	//调试模式
	bool isDebug;
};

