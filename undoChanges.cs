//  https://stackoverflow.com/questions/5466677/undo-changes-in-entity-framework-entities 

Query ChangeTracker of DbContext for dirty items. Set deleted items state to unchanged and added items to detached. For modified items, use original values and set current values of the entry. Finally set state of modified entry to unchanged:
public void RollBack()
{
    var context = DataContextFactory.GetDataContext();
    var changedEntries = context.ChangeTracker.Entries()
        .Where(x => x.State != EntityState.Unchanged).ToList();

    foreach (var entry in changedEntries)
    {
        switch(entry.State)
        {
            case EntityState.Modified:
                entry.CurrentValues.SetValues(entry.OriginalValues);
                entry.State = EntityState.Unchanged;
                break;
            case EntityState.Added:
                entry.State = EntityState.Detached;
                break;
            case EntityState.Deleted:
                entry.State = EntityState.Unchanged;
                break;
        }
    }
 }
 
 // Undo the changes of all entries. 
foreach (DbEntityEntry entry in context.ChangeTracker.Entries()) 
{ 
    switch (entry.State) 
    { 
        // Under the covers, changing the state of an entity from  
        // Modified to Unchanged first sets the values of all  
        // properties to the original values that were read from  
        // the database when it was queried, and then marks the  
        // entity as Unchanged. This will also reject changes to  
        // FK relationships since the original value of the FK  
        // will be restored. 
        case EntityState.Modified: 
            entry.State = EntityState.Unchanged; 
            break; 
        case EntityState.Added: 
            entry.State = EntityState.Detached; 
            break; 
        // If the EntityState is the Deleted, reload the date from the database.   
        case EntityState.Deleted: 
            entry.Reload(); 
            break; 
        default: break; 
    } 
}


 This is an example of what Mrnka is talking about. The following method overwrites an entity's current values with the original values and doesn't call out the database. We do this by making use of the OriginalValues property of DbEntityEntry, and make use of reflection to set values in a generic way. (This works as of EntityFramework 5.0)
/// <summary>
/// Undoes any pending updates 
/// </summary>
public void UndoUpdates( DbContext dbContext )
{
    //Get list of entities that are marked as modified
    List<DbEntityEntry> modifiedEntityList = 
        dbContext.ChangeTracker.Entries().Where(x => x.State == EntityState.Modified).ToList();

    foreach(  DbEntityEntry entity in modifiedEntityList ) 
    {
        DbPropertyValues propertyValues = entity.OriginalValues;
        foreach (String propertyName in propertyValues.PropertyNames)
        {                    
            //Replace current values with original values
            PropertyInfo property = entity.Entity.GetType().GetProperty(propertyName);
            property.SetValue(entity.Entity, propertyValues[propertyName]); 
        }
    }
}