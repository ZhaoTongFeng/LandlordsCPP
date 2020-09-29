#include "Player.h"
#include "BaseCards.h"

Player::Player(const std::string& name, int bal) :
	name(name),
	balance(bal),
	point(0),
	isFinishOpt(false)
{
}

Player::~Player()
{
}

void Player::CreateCards(BaseCards* card)
{
	cards.emplace_back(card);
}

void Player::RemoveCards()
{
}

void Player::ResetAllCardStack()
{
	for (auto& cardStack : cards) {
		cardStack->ReSet();
	}
	point = 0;
}
