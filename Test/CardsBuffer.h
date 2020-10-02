#pragma once

//�ƶ�ջ
class CardsBuffer
{
public:
	CardsBuffer(int size);
	//�������캯�������moveΪ�棬���ڸ��ƹ���֮�����target
	CardsBuffer(CardsBuffer* target, bool move = false);
	virtual ~CardsBuffer() { delete[] buf; }

	bool Push(const int& x);
	bool Get(int& x);

	//��һ���ƶѵ���һ���ƶ�
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

