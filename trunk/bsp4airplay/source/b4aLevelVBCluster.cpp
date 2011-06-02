#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include <IwModelBlock.h>
#include "b4aLevelVBCluster.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLevelVBCluster g_ParseLevelVBCluster;
#endif
}

//Constructor
Cb4aLevelVBCluster::Cb4aLevelVBCluster()
{
	vertexbuffer = 0;
	lastVisibleOnFrame = 0;
}

//Desctructor
Cb4aLevelVBCluster::~Cb4aLevelVBCluster()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aLevelVBCluster::Serialise ()
{
	IwSerialiseUInt32(vertexbuffer);
	subclusters.SerialiseHeader();
	for (uint32 i=0; i<subclusters.size(); ++i)
		subclusters[i].Serialise();

}

void Cb4aLevelVBCluster::Render(Cb4aLevel* l)
{
	if (subclusters.empty())
		return;
	uint32 frameid = l->GetCurrentFrameId();
	if (frameid == lastVisibleOnFrame)
		return;
	lastVisibleOnFrame = frameid;
	for (uint32 i=0; i<subclusters.size(); ++i)
		if (subclusters[i].IsVisible())
			l->ScheduleRender(vertexbuffer, &subclusters[i]);
}
#ifdef IW_BUILD_RESOURCES
Cb4aLevelVBSubcluster* ParseLevelVBCluster::AllocateSubcluster()
{
	_this->subclusters.push_back();
	return &_this->subclusters.back();
}
void* Bsp4Airplay::Cb4aLevelVBClusterFactory()
{
	return &g_ParseLevelVBCluster;
}

// Parse from text file: start block.
void  ParseLevelVBCluster::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateCluster();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLevelVBCluster::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	
	if (!strcmp("vertexbuffer", pAttrName))
	{
		pParser->ReadUInt32(&_this->vertexbuffer);
		return true;
	}
	if (!strcmp("num_subclusters", pAttrName))
	{
		int32 n;
		pParser->ReadInt32(&n);
		_this->subclusters.set_capacity(n);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLevelVBCluster::ParseClose(CIwTextParserITX* pParser)
{
}
#endif