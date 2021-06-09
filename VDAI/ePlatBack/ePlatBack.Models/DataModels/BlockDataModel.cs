using System;
using System.IO;
using System.Web;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using ePlatBack.Models.Utils;
using System.Collections.Generic;
using ePlatBack.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace ePlatBack.Models.DataModels
{
    public class BlockDataModel
    {
        ePlatEntities db = new ePlatEntities();
        public static UserSession session = new UserSession();

        public IEnumerable<BlockItem> GetBlocksList()
        {
            List<BlockItem> blocks = new List<BlockItem>();
            List<KeyValuePair<string, long>> listTerminals = new List<KeyValuePair<string, long>>();

            string terminals = session.Terminals;
            if (terminals != "")
            {
                foreach (var i in terminals.Split(','))
                    listTerminals.Add(new KeyValuePair<string, long>("", Int64.Parse(i)));
            }
            var arrayTerminals = listTerminals.Select(m => m.Value);
            var blocksQ = from b in db.tblBlocks
                          where arrayTerminals.Contains(b.terminalID)
                          select b;

            foreach (var block in blocksQ)
            {
                blocks.Add(new BlockItem()
                {
                    BlockItem_BlockID = block.blockID,
                    BlockItem_TerminalName = block.tblTerminals.terminal,
                    BlockItem_Block = block.block,
                });
            }

            return blocks;
        }

        public AttemptResponse SaveBlockItem(BlockItem block)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (block.BlockItem_BlockID != 0)
            {
                try
                {
                    var query = db.tblBlocks.Single(m => m.blockID == block.BlockItem_BlockID);
                    query.block = block.BlockItem_Block;
                    query.terminalID = block.BlockItem_TerminalID;
                    query.general = block.BlockItem_General;
                    query.frame = block.BlockItem_Frame == null ? "" : block.BlockItem_Frame;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Block Updated";
                    response.ObjectID = query.blockID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Block NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblBlocks newBlock = new tblBlocks();
                    newBlock.block = block.BlockItem_Block;
                    newBlock.terminalID = block.BlockItem_TerminalID;
                    newBlock.general = block.BlockItem_General;
                    newBlock.frame = block.BlockItem_Frame == null ? "" : block.BlockItem_Frame;

                    db.tblBlocks.AddObject(newBlock);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Block Saved";
                    response.ObjectID = newBlock.blockID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Block NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public AttemptResponse DeleteBlockItem(long blockID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var block = db.tblBlocks.Single(m => m.blockID == blockID);
                db.tblBlocks.DeleteObject(block);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Block Deleted";
                response.ObjectID = blockID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Block NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public AttemptResponse DeleteBlockDescription(long blockDescID)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            try
            {
                var block = db.tblBlockDescriptions.Single(m => m.blockDescriptionID == blockDescID);
                db.tblBlockDescriptions.DeleteObject(block);
                db.SaveChanges();
                response.Type = Attempt_ResponseTypes.Ok;
                response.Message = "Block Description Deleted";
                response.ObjectID = blockDescID;
                return response;
            }
            catch (Exception ex)
            {
                response.Type = Attempt_ResponseTypes.Error;
                response.Message = "Block Description NOT Deleted";
                response.ObjectID = 0;
                response.Exception = ex;
                return response;
            }
        }

        public BlockItem GetBlockItem(long blockID)
        {
            BlockItem block = new BlockItem();

            var blockQ = (from b in db.tblBlocks
                          where b.blockID == blockID
                          select b).FirstOrDefault();

            if (blockQ != null)
            {
                block.BlockItem_BlockID = blockQ.blockID;
                block.BlockItem_TerminalID = blockQ.terminalID;
                block.BlockItem_TerminalName = blockQ.tblTerminals.terminal;
                block.BlockItem_Block = blockQ.block;
                block.BlockItem_General = blockQ.general;
                block.BlockItem_Frame = blockQ.frame;
                List<BlockDescription> descriptions = new List<BlockDescription>();
                foreach (tblBlockDescriptions desc in blockQ.tblBlockDescriptions)
                {
                    descriptions.Add(new BlockDescription() { 
                       BlockDescription_ID = desc.blockDescriptionID,
                       BlockDescription_BlockID = desc.blockID,
                       BlockDescription_Content = desc.content_,
                       BlockDescription_Culture = desc.culture,
                       BlockDescription_Language = db.tblLanguages.Where(x => x.culture == desc.culture).FirstOrDefault().language
                    });
                }
                block.BlockItem_Descriptions = descriptions;
            }

            return block;
        }

        public AttemptResponse SaveBlockDescriptionItem(BlockDescription desc)
        {
            ePlatEntities db = new ePlatEntities();
            AttemptResponse response = new AttemptResponse();
            if (desc.BlockDescription_ID != 0)
            {
                try
                {
                    var query = db.tblBlockDescriptions.Single(m => m.blockDescriptionID == desc.BlockDescription_ID);
                    query.blockDescriptionID = desc.BlockDescription_ID;
                    query.blockID = desc.BlockDescription_BlockID;
                    query.content_ = desc.BlockDescription_Content;
                    query.culture = desc.BlockDescription_Culture;
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "BlockDescription Updated";
                    response.ObjectID = query.blockDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Block Description NOT Updated";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
            else
            {
                try
                {
                    tblBlockDescriptions newBlock = new tblBlockDescriptions();
                    newBlock.blockDescriptionID = desc.BlockDescription_ID;
                    newBlock.blockID = desc.BlockDescription_BlockID;
                    newBlock.content_ = desc.BlockDescription_Content;
                    newBlock.culture = desc.BlockDescription_Culture;

                    db.tblBlockDescriptions.AddObject(newBlock);
                    db.SaveChanges();
                    response.Type = Attempt_ResponseTypes.Ok;
                    response.Message = "Block Description Saved";
                    response.ObjectID = newBlock.blockDescriptionID;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Type = Attempt_ResponseTypes.Error;
                    response.Message = "Block Description NOT Saved";
                    response.ObjectID = 0;
                    response.Exception = ex;
                    return response;
                }
            }
        }

        public static string GetBlockDescription(long blockID)
        {
            ePlatEntities db = new ePlatEntities();
            string description = "";
            string culture = Utils.GeneralFunctions.GetCulture();
            var blockDescription = (from d in db.tblBlockDescriptions
                                   where d.blockID == blockID
                                   && d.culture == culture
                                   select d.content_).FirstOrDefault();

            if (blockDescription != null)
            {
                description = blockDescription;
            }

            return description;
        }
    }
}
