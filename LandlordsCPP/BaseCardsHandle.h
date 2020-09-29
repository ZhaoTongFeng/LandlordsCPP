#pragma once

//发牌基本规则
class BaseCardsHandle{
public:
	BaseCardsHandle(const int maxCount);
	virtual ~BaseCardsHandle();

	//洗牌
	virtual void Shuffle() = 0;

	//给一幅手牌发一张牌
	void HandCard(class BaseCards* c);
	//给一个牌堆数组的一个元素发一张底牌
	void HandCard(int& cardsEle);

	virtual void ComputeAttribute(){}

	//牌堆是否为空
	bool isEmpty() { return cards_ptr == -1; }

public:
	//获取牌堆
	const int* GetCards() { return cards; }

	//当前牌堆剩余
	int GetCardsCount() { return cards_ptr + 1; }

protected:

	//牌堆
	int* cards;

	//牌堆栈顶指针
	int cards_ptr;

	int MAX_CARDS_COUNT;
};

