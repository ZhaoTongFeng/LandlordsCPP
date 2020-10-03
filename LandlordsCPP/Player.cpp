#include "Player.h"

#include "CardsBuffer.h"

Player::Player(const std::string& name, int bal) :
	name(name),
	balance(bal),

	isFinishOpt(false)
{
}

Player::~Player()
{
	for (int i = 0; i < cards.size(); i++) {
		delete cards[i];
	}
}

void Player::CreateCards(CardsBuffer* card)
{
	cards.emplace_back(card);
}

void Player::ResetAllCardStack()
{
	for (auto& cardStack : cards) {
		cardStack->MakeEmpty();
	}
}
