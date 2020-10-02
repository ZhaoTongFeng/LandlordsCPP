#pragma once
#include <string>
#include <vector>


class Player
{
public:
	Player(const std::string& name, int bal);
	~Player();



	//手牌容器操作
	class CardsBuffer* GetCardStack(int i = 0) { return cards[i]; }
	std::vector<class CardsBuffer*>& GetCards(){ return cards; }
	void CreateCards(class CardsBuffer* card);
	void RemoveCards();
	void ResetAllCardStack();

	//队伍
	int team = 0;

	bool isFinishOpt;

	
	//Getter/Setter
	void ChangeBalance(int n) { balance += n; }
	const int& GetBalance()const { return balance; }

	const std::string& GetName() const { return name; }
	void SetName(const std::string& newname) { name = newname; }
private:
//全局数据
	//玩家昵称
	std::string name;

	//余额
	int balance;

//对局数据
	//手牌，由游戏规则制定手牌数量，切换游戏时需要重置
	std::vector<class CardsBuffer*> cards;

	



};

