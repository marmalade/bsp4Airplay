#include <IwTextParserITX.h>
#include <algorithm>
#include <b4aLevelVB.h>
#include <b4aLevel.h>

using namespace Bsp4Airplay;

namespace Bsp4Airplay
{
#ifdef IW_BUILD_RESOURCES
	ParseLevelVertexBuffer g_parseLevelVertexBuffer;
#endif
	bool SortByMaterial(Cb4aLevelVBSubcluster*a,Cb4aLevelVBSubcluster*b)
	{
		return a->GetMaterial() < b->GetMaterial();
	}
}


Cb4aLevelVertexBuffer::Cb4aLevelVertexBuffer()
{
}

Cb4aLevelVertexBuffer::~Cb4aLevelVertexBuffer()
{
	if (positionsStream.IsSet())
	{
		positionsStream.Free();
		normalsStream.Free();
		uv0Stream.Free();
		uv1Stream.Free();
		coloursStream.Free();
	}
}
void Cb4aLevelVertexBuffer::Serialise()
{
	if (positionsStream.IsSet())
		IwAssertMsg(BSP,false,("Can't serialise uploaded stream"));

	positions.SerialiseHeader();
	for (uint32 i=0; i<positions.size(); ++i)
		positions[i].Serialise();
	normals.SerialiseHeader();
	for (uint32 i=0; i<normals.size(); ++i)
		normals[i].Serialise();
	uv0s.SerialiseHeader();
	for (uint32 i=0; i<uv0s.size(); ++i)
		uv0s[i].Serialise();
	colours.SerialiseHeader();
	for (uint32 i=0; i<colours.size(); ++i)
		colours[i].Serialise();
	map.SerialiseHeader();
	for (uint32 i=0; i<map.size(); ++i)
	{
		IwSerialiseUInt16(map[i].indices[0], 5);
	}
}
void Cb4aLevelVertexBuffer::ScheduleCluster(Cb4aLevelVBSubcluster* cluster)
{
	renderQueue.push_back(cluster);
}
void Cb4aLevelVertexBuffer::FlushQueueDynamicBlock(Cb4aLevel* l,uint32 from, uint32 end)
{
	uint32 totalIndices = 0;
	for (uint32 i=from; i<end; ++i)
	{
		totalIndices +=renderQueue[i]->GetIndices().size();
	}
	if (totalIndices == 0)
		return;
	l->BindMaterial(renderQueue[from]->GetMaterial());
	CIwSVec3* temp_positions = IW_GX_ALLOC(CIwSVec3,totalIndices);
	//CIwSVec3* temp_normals = IW_GX_ALLOC(CIwSVec3,totalIndices);
	CIwSVec2* temp_uv0s = IW_GX_ALLOC(CIwSVec2,totalIndices);
	CIwSVec2* temp_uv1s = IW_GX_ALLOC(CIwSVec2,totalIndices);
	uint16* indinces = IW_GX_ALLOC(uint16,totalIndices);
	int offset = 0;
	for (uint32 i=from; i<end; ++i)
	{
		const CIwArray<uint16>& indices = renderQueue[i]->GetIndices();
		for (uint32 i=0; i<indices.size(); ++i)
		{
			temp_positions[offset] = GetPosition(indices[i]);
			//temp_normals[offset] = GetNormal(indices[i]);
			temp_uv0s[offset] = GetUV0(indices[i]);
			temp_uv1s[offset] = GetUV1(indices[i]);
			indinces[offset] = (uint16)offset;
			++offset;
		}
	}

	IwGxSetVertStream(temp_positions, totalIndices);
	//IwGxSetNormStream(temp_normals, totalIndices);
	IwGxSetNormStream(0);
	IwGxSetUVStream(temp_uv0s, 0);
	IwGxSetUVStream(temp_uv1s, 1);
	//IwGxSetColStream(temp_colours, );
	IwGxSetColStream(0);

	IwGxDrawPrims(IW_GX_TRI_LIST,indinces,totalIndices);

	PostRender();
}
void Cb4aLevelVertexBuffer::FlushQueueBlock(Cb4aLevel* l,uint32 from, uint32 end)
{
	uint32 totalIndices = 0;
	for (uint32 i=from; i<end; ++i)
	{
		totalIndices +=renderQueue[i]->GetIndices().size();
	}
	if (totalIndices == 0)
		return;
	l->BindMaterial(renderQueue[from]->GetMaterial());
	uint16* ptr = IW_GX_ALLOC(uint16,totalIndices);
	uint16* cur = ptr;
	for (uint32 i=from; i<end; ++i)
	{
		const CIwArray<uint16>& indices = renderQueue[i]->GetIndices();
		memcpy(cur,&indices[0],indices.size()*2);
		cur += indices.size();
	}
	IwGxDrawPrims(IW_GX_TRI_LIST,ptr,totalIndices);
}
void Cb4aLevelVertexBuffer::Flush(Cb4aLevel* l)
{
	if (renderQueue.empty())
		return;

	//This sould not happen since materials are sorted in tool
	uint32 prevVB=0;
	uint32 prevMat=0;
	for (CIwArray<Cb4aLevelVBSubcluster*>::iterator q=renderQueue.begin();q!=renderQueue.end();++q)
	{
		if (prevVB > (*q)->vb)
		{
			std::sort(renderQueue.begin(),renderQueue.end(), SortByMaterial);
			break;
		}
		if (prevVB < (*q)->vb)
		{
			prevVB = (*q)->vb;
			prevMat=0;
		}
		if (prevMat > (*q)->material)
		{
			std::sort(renderQueue.begin(),renderQueue.end(), SortByMaterial);
			break;
		}
		if (prevMat < (*q)->material)
			prevMat = (*q)->material;
	}

	//std::sort(renderQueue.begin(),renderQueue.end(), SortByMaterial);

	if (false)
	{
		PreRender();

		uint32 totalItems = renderQueue.size();
		uint32 start = 0;
		while (start< totalItems)
		{
			uint32 end = start;
			while (end < totalItems && renderQueue[end]->GetMaterial() == renderQueue[start]->GetMaterial()) ++end;
			FlushQueueBlock(l,start,end);
			start = end;
		}


		PostRender();
	}
	else
	{
		uint32 totalItems = renderQueue.size();
		uint32 start = 0;
		while (start< totalItems)
		{
			uint32 end = start;
			while (end < totalItems && renderQueue[end]->GetMaterial() == renderQueue[start]->GetMaterial()) ++end;
			FlushQueueDynamicBlock(l,start,end);
			start = end;
		}
	}
	
}

void Cb4aLevelVertexBuffer::PreRender()
{
	if (!positionsStream.IsSet())
	{
		if (map.empty())
			return;
		CIwSVec3* p = new CIwSVec3[map.size()];
		CIwSVec3* n = new CIwSVec3[map.size()];
		CIwSVec2* uv0 = new CIwSVec2[map.size()];
		CIwSVec2* uv1 = new CIwSVec2[map.size()];
		CIwColour* col = new CIwColour[map.size()];
		for (uint32 i=0;i<map.size();++i)
		{
			p[i] = GetPosition(i);
			n[i] = GetNormal(i);
			uv0[i] = GetUV0(i);
			uv1[i] = GetUV1(i);
			col[i] = GetColour(i);
		}
		positionsStream.Set(CIwGxStream::SVEC3, p, map.size(), 0);
		positionsStream.Upload(true, true);
		normalsStream.Set(CIwGxStream::SVEC3, n, map.size(), 0);
		normalsStream.Upload(true, true);
		uv0Stream.Set(CIwGxStream::SVEC2, uv0, map.size(), 0);
		uv0Stream.Upload(true, true);
		uv1Stream.Set(CIwGxStream::SVEC2, uv1, map.size(), 0);
		uv1Stream.Upload(true, true);
		coloursStream.Set(CIwGxStream::COLOUR, col, map.size(), 0);
		coloursStream.Upload(true,true);
	}
	IwGxSetVertStreamWorldSpace(positionsStream);
	IwGxSetNormStream(normalsStream);
	IwGxSetUVStream(uv0Stream, 0);
	IwGxSetUVStream(uv1Stream, 1);
	//IwGxSetColStream(coloursStream);

	//IwGxSetVertStream(&positions.front(), positions.size());
	//IwGxSetNormStream(&normals.front(), normals.size());
	//IwGxSetUVStream(&uv0s.front(), 0);
	//IwGxSetUVStream(&uv1s.front(), 1);
	//IwGxSetColStream(&colours.front(), colours.size());
}

void Cb4aLevelVertexBuffer::PostRender()
{
	static CIwSVec3 fakeStream[1];
	IwGxSetVertStream(&fakeStream[0],1);
	IwGxSetNormStream(0,0);
	IwGxSetUVStream(0,0);
	IwGxSetUVStream(0, 1);
	IwGxSetColStream(0,0);
}

#ifdef IW_BUILD_RESOURCES
void* Bsp4Airplay::Cb4aLevelVertexBufferFactory()
{
	return &g_parseLevelVertexBuffer;
}

// Parse from text file: start block.
void  ParseLevelVertexBuffer::ParseOpen (CIwTextParserITX *pParser)
{
	CIwManaged::ParseOpen(pParser);
	Cb4aLevel* level = dynamic_cast<Cb4aLevel*>(pParser->GetObject(-1));
	_this = level->AllocateLevelVertexBuffer();
}

// function invoked by the text parser when parsing attributes for objects of this type
bool ParseLevelVertexBuffer::ParseAttribute(CIwTextParserITX *pParser, const char *pAttrName)
{
	if (!strcmp("num_pos", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		_this->positions.set_capacity(num_verts);
		return true;
	}
	if (!strcmp("num_n", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		_this->normals.set_capacity(num_verts);
		return true;
	}
	if (!strcmp("num_uvs", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		_this->uv0s.set_capacity(num_verts);
		return true;
	}
	if (!strcmp("num_cols", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		_this->colours.set_capacity(num_verts);
		return true;
	}
	if (!strcmp("num_vertices", pAttrName))
	{
		uint32 num_verts;
		pParser->ReadUInt32(&num_verts);
		_this->map.set_capacity(num_verts);
		return true;
	}
	if (!strcmp("v", pAttrName))
	{
		CIwSVec3 v;
		pParser->ReadInt16Array(&v.x,3);
		_this->positions.push_back(v);
		return true;
	}
	if (!strcmp("vn", pAttrName))
	{
		CIwSVec3 vn;
		pParser->ReadInt16Array(&vn.x,3);
		_this->normals.push_back(vn);
		return true;
	}
	if (!strcmp("uv", pAttrName))
	{
		CIwSVec2 uv0;
		pParser->ReadInt16Array(&uv0.x,2);
		_this->uv0s.push_back(uv0);
		return true;
	}
	if (!strcmp("col", pAttrName))
	{
		uint8 col[4];
		pParser->ReadUInt8Array(&col[0],4);
		CIwColour c;
		c.Set(col[0],col[1],col[2],col[3]);
		_this->colours.push_back(c);
		return true;
	}
	if (!strcmp("i", pAttrName))
	{
		_this->map.push_back();
		pParser->ReadUInt16Array(&_this->map.back().indices[0],5);
		return true;
	}
	return CIwManaged::ParseAttribute(pParser,pAttrName);
}

// function invoked by the text parser when the object definition end is encountered
void ParseLevelVertexBuffer::ParseClose(CIwTextParserITX* pParser)
{
}
#endif