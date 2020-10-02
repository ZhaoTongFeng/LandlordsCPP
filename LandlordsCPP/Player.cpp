#include "Player.h"
#include "BaseCards.h"
#include "CardsBuffer.h"

Player::Player(const std::string& name, int bal) :
	name(name),
	balance(bal),

	isFinishOpt(false)
{
}

Player::~Player()
{
}

void Player::CreateCards(CardsBuffer* card)
{
	cards.emplace_back(card);
}

void Player::RemoveCards()
{
}

void Player::ResetAllCardStack()
{
	for (auto& cardStack : cards) {
		cardStack->MakeEmpty();
	}
}
