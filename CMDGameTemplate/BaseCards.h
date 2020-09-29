#pragma once
#include <string>

//一幅手牌基本信息
//手牌特征
class BaseCards
{
public:
	//构造函数初始化成员变量
	BaseCards(const int num_cards);

	virtual ~BaseCards();

	//重置手牌
	virtual void ReSet();
	virtual void ComputeAttribute() {};

	//获取牌名
	static std::string GetCardName(int n);
	//获取牌面不含大小鬼
	static std::string GetCardNameWithoutJoker(int n);

	//添加一张手牌
	void PushCard(int n);
	const int* GetCards() { return cards; }

	int& GetCardsCount() { return cards_count; }

	bool IsFinish() { return isFinish; }
	void SetIsFinish(bool fin) { isFinish = fin; }

	//排序，不看花色只看点数，先扫描一遍查看是否有大小王，如果有放到最左侧，再对剩下的牌进行自然排序

	//出牌之后将右边的全部往左移

	//出牌时，将选中的牌放到一个列表中，检测是否符合牌型，并记录牌型

protected:
	//手牌
	int* cards;

	//手牌数量
	int cards_count;

	//本手牌操作已结束
	bool isFinish;
};

