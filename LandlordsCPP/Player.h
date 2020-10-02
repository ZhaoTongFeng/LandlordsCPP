#pragma once
#include <string>
#include <vector>


class Player
{
public:
	Player(const std::string& name, int bal);
	~Player();



	//������������
	class CardsBuffer* GetCardStack(int i = 0) { return cards[i]; }
	std::vector<class CardsBuffer*>& GetCards(){ return cards; }
	void CreateCards(class CardsBuffer* card);
	void RemoveCards();
	void ResetAllCardStack();

	//����
	int team = 0;

	bool isFinishOpt;

	
	//Getter/Setter
	void ChangeBalance(int n) { balance += n; }
	const int& GetBalance()const { return balance; }

	const std::string& GetName() const { return name; }
	void SetName(const std::string& newname) { name = newname; }
private:
//ȫ������
	//����ǳ�
	std::string name;

	//���
	int balance;

//�Ծ�����
	//���ƣ�����Ϸ�����ƶ������������л���Ϸʱ��Ҫ����
	std::vector<class CardsBuffer*> cards;

	



};

