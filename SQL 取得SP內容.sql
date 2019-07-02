select definition
from sys.objects a
inner join sys.sql_modules b on a.object_id = b.object_id
where name = 'UDEBT_DAILY_TRANSFER_IDRP_MDRP_SP'
