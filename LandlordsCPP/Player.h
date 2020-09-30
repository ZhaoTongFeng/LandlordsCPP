#pragma once
#include <string>
#include <vector>


class Player
{
public:
	Player(const std::string& name, int bal);
	~Player();

	//Getter/Setter
	bool ChangeBalance(int n) { balance += n; }
	const int& GetBalance()const { return balance; }

	const std::string& GetName() const { return name; }
	void SetName(const std::string& newname) { name = newname; }

	//������������
	class BaseCards* GetCardStack(int i = 0) { return cards[i]; }
	std::vector<class BaseCards*>& GetCards(){ return cards; }
	
	void CreateCards(class BaseCards* card);
	void RemoveCards();

	
	void ResetAllCardStack();

	//���Ƶ���
	int point;

	bool isFinishOpt;

	

private:
//ȫ������
	//����ǳ�
	std::string name;

	//���
	int balance;

//�Ծ�����
	//���ƣ�����Ϸ�����ƶ������������л���Ϸʱ��Ҫ����
	std::vector<class BaseCards*> cards;

	



};

