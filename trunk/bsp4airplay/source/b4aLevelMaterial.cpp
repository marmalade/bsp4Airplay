#include <IwTextParserITX.h>
#include <IwResManager.h>
#include <IwResGroup.h>
#include <IwModelBlock.h>
#include "b4aLevelMaterial.h"
#include "b4aLevel.h"

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLevelMaterial g_parseLevelMaterial;
#endif
}

//Constructor
Cb4aLevelMaterial::Cb4aLevelMaterial()
{
	texture0Hash = 0;
	texture1Hash = 0;
	texture0 = 0;
	texture1 = 0;
	transparent = false;
	sky = false;

	texturesResolved = false;
}

//Desctructor
Cb4aLevelMaterial::~Cb4aLevelMaterial()
{
}

// Reads/writes a binary file using IwSerialise interface. 
void  Cb4aLevelMaterial::Serialise ()
{
	IwSerialiseUInt32(texture0Hash);
	IwSerialiseUInt32(texture1Hash);
	IwSerialiseBool(sky);
	IwSerialiseBool(transparent);

}

void Cb4aLevelMaterial::Bind(Cb4aLevel* l)
{
	if (!texturesResolved)
	{
		texturesResolved = true;
		if (texture0Hash)
		{
			texture0 = (CIwTexture*)IwGetResManager()->GetResHashed(texture0Hash, "CIwTexture", IW_RES_PERMIT_NULL_F | IW_RES_SEARCH_ALL_F);
			if (!texture0)
				texture0 = l->GetDefaultTextrure();
		}
		if (texture1Hash)
		{
			texture1 = (CIwTexture*)IwGetResManager()->GetResHashed(texture1Hash, "CIwTexture", IW_RES_PERMIT_NULL_F | IW_RES_SEARCH_ALL_F);
		}
	}
	CIwMaterial* m = IW_GX_ALLOC_MATERIAL();
	if (texture0)
	{
		m->SetTexture(texture0,0);
		m->SetModulateMode(CIwMaterial::MODULATE_NONE);
	}
	else
	{
		m->SetColDiffuse(IwGxGetColFixed(IW_GX_COLOUR_WHITE));
		m->SetModulateMode(CIwMaterial::MODULATE_RGB);
	}
	if (transparent)
	{
		m->SetAlphaMode(CIwMaterial::ALPHA_BLEND);
		m->SetDepthWriteMode(CIwMaterial::DEPTH_WRITE_DISABLED);
	}
	if (texture1)
	{
		m->SetTexture(texture1,1);
		m->SetBlendMode(CIwMaterial::BLEND_MODULATE);
	}
	IwGxSetMaterial(m);
}

#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aLevelMaterialFactory()
{
	return &g_parseLevelMaterial;
}

// Parse from text file: start block.
void  ParseLevelMaterial::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateLevelMaterial();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLevelMaterial::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("texture0", pAttrName))
	{
		pParser->ReadStringHash(&_this->texture0Hash);
		return true;
	}
	if (!strcmp("texture1", pAttrName))
	{
		pParser->ReadStringHash(&_this->texture1Hash);
		return true;
	}
	if (!strcmp("sky", pAttrName))
	{
		pParser->ReadBool(&_this->sky);
		return true;
	}
	if (!strcmp("transparent", pAttrName))
	{
		pParser->ReadBool(&_this->transparent);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLevelMaterial::ParseClose(CIwTextParserITX* pParser)
{
}
#endif