#include "Player.h"
#include "BaseCards.h"

Player::Player(const std::string& name, int bal):
	name(name),
	balance(bal)
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

void Player::ClearAllCardStack()
{
	for (auto& cardStack : cards) {
		cardStack->ReSet();
	}
}
