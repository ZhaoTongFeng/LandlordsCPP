#pragma once

//牌堆栈
class CardsBuffer
{
public:
	CardsBuffer(int size);
	//拷贝构造函数：如果move为真，则在复制过来之后清空target
	CardsBuffer(CardsBuffer* target, bool move = false);
	virtual ~CardsBuffer() { delete[] buf; }

	bool Push(const int& x);
	bool Get(int& x);

	//从一个牌堆到另一个牌堆
	bool Pop(int& x);
	bool Pop(CardsBuffer* target);
	bool Pop(CardsBuffer* target, int n);

	bool IsEmpty()const { return top == -1; }
	bool IsFull()const { return top == maxSize - 1; }
	void MakeEmpty() { top = -1; }

	//Getter/Setter
	int* GetBuf() { return buf; }
	int GetSize()const { return top + 1; }
	int GetMaxSize()const { return maxSize; }

private:
	int* buf;
	int top, maxSize;
};

