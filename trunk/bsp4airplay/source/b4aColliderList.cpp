#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aColliderList.h"

using namespace Bsp4Airplay;	
Cb4aColliderList::Cb4aColliderList()
{
}
Cb4aColliderList::~Cb4aColliderList()
{
	Delete();
}
void Cb4aColliderList::SerialisePtrs()
{
	for (uint32 i=0; i<m_List.size(); ++i)
	{
		if (IwSerialiseIsReading())
		{
			CIwManaged* m;
			IwSerialiseManagedObject(m);
			m_List[i] = dynamic_cast<Ib4aCollider*>(m);
		}
		else
		{
			CIwManaged* m = dynamic_cast<CIwManaged*>(m_List[i]);
			IwSerialiseManagedObject(m);
		}
	}
}
void	Cb4aColliderList::Delete()
{
	for (uint32 i=0; i<m_List.size(); ++i)
	{
		delete m_List[i];
		m_List[i] = 0;
	}
	m_List.clear();
}
void Cb4aColliderList::Serialise()
{
	SerialiseHeader();
	SerialisePtrs();
}