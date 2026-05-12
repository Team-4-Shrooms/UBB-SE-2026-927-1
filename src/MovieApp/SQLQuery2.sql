-- 1. Adaugă coloana Capacity manual (dacă a lipsit din migrare)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Events') AND name = 'Capacity')
BEGIN
    ALTER TABLE Events ADD Capacity INT NOT NULL DEFAULT 100;
END

-- 2. Pune bani în cont
UPDATE Users SET Balance = 10000 WHERE Username = 'admin';

-- 3. Setează capacitatea (dacă există coloana deja)
UPDATE Events SET Capacity = 100;

-- 1. Pune bani la TOȚI utilizatorii (pentru a putea cumpăra orice)
UPDATE Users SET Balance = 20000;

-- 2. Asigură-te că Evenimentele au capacitate (pentru a nu mai fi Sold Out)
UPDATE Events SET Capacity = 100;


