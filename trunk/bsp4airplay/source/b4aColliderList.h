#pragma once
#include <IwManagedList.h>
#include <Ib4aCollider.h>
namespace Bsp4Airplay
{
	class Cb4aColliderList
	{
		CIwArray<Ib4aCollider*> m_List;
		public:
		typedef CIwArray<Ib4aCollider*>::const_iterator const_iterator;

		//Constructor
		Cb4aColliderList();
		//Desctructor
		inline const_iterator begin() const {return m_List.begin();}
		inline const_iterator end() const {return m_List.end();}
		virtual ~Cb4aColliderList();
		inline void push_back(Ib4aCollider* c){m_List.push_back(c);}
		inline uint32 GetSize() const {return (uint32)m_List.size();}
		inline void SerialiseHeader() {m_List.SerialiseHeader();}
		inline const Ib4aCollider*operator[](uint32 i) const {return m_List[i];}
		void	Delete();
		void	SerialisePtrs();
		void	Serialise();
	};
}