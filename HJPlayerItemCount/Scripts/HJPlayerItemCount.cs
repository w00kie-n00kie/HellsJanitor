using HarmonyLib;

public class HJPlayerItemCount : PlayerItemCount
{
	public override bool IsValid(MinEventParams _params)
	{
		if (!ParamsValid(_params))
		{
			return false;
		}

		var itemName = AccessTools.Field(typeof(PlayerItemCount), "item_name").GetValue(this) as string;
		ItemValue item = null;

		if (itemName != null && item == null)
		{
			item = ItemClass.GetItem(itemName, true);
		}

		if (item == null)
		{
			return false;
		}

		int num = target.inventory.GetItemCount(item, false, -1, -1, false);
		num += target.bag.GetItemCount(item, -1, -1, false);

		if (invert)
		{
			return !compareValues(num, operation, value);
		}

		return compareValues(num, operation, value);
	}
}
