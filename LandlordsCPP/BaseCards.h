#pragma once
#include <string>

//一幅手牌基本信息
//手牌特征
class BaseCards
{
public:
	BaseCards() {};
	//构造函数初始化成员变量
	BaseCards(const int num_cards);
	BaseCards(BaseCards* target);

	virtual ~BaseCards();

	//重置手牌
	virtual void ReSet();

	//更新一些属性
	virtual void ComputeAttribute() {};


	//获取牌面不含大小鬼
	static std::string GetCardNameWithoutJoker(int n);

	//添加一张手牌
	void PushCard(int n);


	int* GetCards() { return cards; }

	int& GetCardsCount() { return cards_count; }



	//排序，不看花色只看点数，先扫描一遍查看是否有大小王，如果有放到最左侧，再对剩下的牌进行自然排序
	void SortCards();
	

protected:
	//手牌
	int* cards = nullptr;

	//手牌数量
	int cards_count = 0;
};

