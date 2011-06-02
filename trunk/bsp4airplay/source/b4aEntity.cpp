#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include "b4aEntity.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseEntity g_parseEntity;
#endif
}

//Constructor
Cb4aEntity::Cb4aEntity()
{
}

//Desctructor
Cb4aEntity::~Cb4aEntity()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aEntity::Serialise ()
{
	if (IwSerialiseIsReading())
	{
		uint32 strLen;
		IwSerialiseUInt32(strLen);
		
		classname.resize(strLen,' ');
		IwSerialiseChar(classname[0],strLen);
	}
	else
	{
		uint32 strLen = classname.length();
		IwSerialiseUInt32(strLen);
		IwSerialiseChar(classname[0],strLen);
	}
	origin.Serialise();
}
#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aEntityFactory()
{
	return &g_parseEntity;
}

// Parse from text file: start block.
void  ParseEntity::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateEntity();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseEntity::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("classname", pAttrName))
	{
		char* s = pParser->ReadString(false);
		_this->classname = s;
		delete s;
		return true;
	}
	if (!strcmp("origin", pAttrName))
	{
		float n [3];
		pParser->ReadFloatArray(&n[0],3);
		_this->origin = CIwVec3((int)n[0],(int)n[1],(int)n[2]);
		return true;
	}
	char* s = pParser->ReadString(false);
	//_this->classname = s;
	delete s;
	return true;
}

// function invoked by the text parser when the object definition end is encountered
void ParseEntity::ParseClose(CIwTextParserITX* pParser)
{
}
#endif