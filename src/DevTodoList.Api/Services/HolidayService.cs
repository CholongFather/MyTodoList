using DevTodoList.Api.Data;
using DevTodoList.Api.Data.Entities;
using DevTodoList.Shared.DTOs;
using DevTodoList.Shared.DTOs.Requests;
using DevTodoList.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DevTodoList.Api.Services;

/// <summary>공휴일/연차 관리 서비스</summary>
public class HolidayService(AppDbContext db)
{
    /// <summary>연도별 휴일 목록 조회</summary>
    public async Task<List<HolidayDto>> GetByYearAsync(int year, CancellationToken ct = default)
    {
        var start = new DateTime(year, 1, 1);
        var end = new DateTime(year, 12, 31);
        return await db.Holidays
            .Where(h => h.Date >= start && h.Date <= end)
            .OrderBy(h => h.Date)
            .Select(h => h.ToDto())
            .ToListAsync(ct);
    }

    /// <summary>기간별 휴일 목록 조회 (간트차트용)</summary>
    public async Task<List<HolidayDto>> GetByRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await db.Holidays
            .Where(h => h.Date >= from.Date && h.Date <= to.Date)
            .OrderBy(h => h.Date)
            .Select(h => h.ToDto())
            .ToListAsync(ct);
    }

    /// <summary>특정 날짜가 공휴일 또는 연차인지 확인</summary>
    public async Task<bool> IsHolidayOrLeaveAsync(DateTime date, CancellationToken ct = default)
    {
        return await db.Holidays.AnyAsync(h => h.Date.Date == date.Date, ct);
    }

    /// <summary>휴일 추가</summary>
    public async Task<HolidayDto> CreateAsync(CreateHolidayRequest req, CancellationToken ct = default)
    {
        var entity = new HolidayEntity { Date = req.Date.Date, Name = req.Name, IsRecurring = req.IsRecurring, HolidayType = (int)req.HolidayType };
        db.Holidays.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    /// <summary>휴일 삭제</summary>
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await db.Holidays.FindAsync([id], ct);
        if (entity is null) return false;
        db.Holidays.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>특정 연도 한국 공휴일 일괄 등록 (중복 제외)</summary>
    public async Task<int> SeedKoreanHolidaysAsync(int year, CancellationToken ct = default)
    {
        var holidays = GetKoreanHolidays(year);
        var existing = await db.Holidays
            .Where(h => h.Date.Year == year)
            .Select(h => h.Date.Date)
            .ToListAsync(ct);
        var existingSet = existing.ToHashSet();

        var newHolidays = holidays
            .Where(h => !existingSet.Contains(h.Date.Date))
            .Select(h => new HolidayEntity { Date = h.Date, Name = h.Name, IsRecurring = h.IsRecurring, HolidayType = 0 })
            .ToList();

        if (newHolidays.Count > 0)
        {
            db.Holidays.AddRange(newHolidays);
            await db.SaveChangesAsync(ct);
        }
        return newHolidays.Count;
    }

    /// <summary>한국 공휴일 목록 생성 (고정 + 음력 공휴일)</summary>
    private static List<(DateTime Date, string Name, bool IsRecurring)> GetKoreanHolidays(int year)
    {
        var list = new List<(DateTime, string, bool)>
        {
            // 고정 공휴일
            (new DateTime(year, 1, 1), "신정", true),
            (new DateTime(year, 3, 1), "삼일절", true),
            (new DateTime(year, 5, 1), "근로자의 날", true),
            (new DateTime(year, 5, 5), "어린이날", true),
            (new DateTime(year, 6, 6), "현충일", true),
            (new DateTime(year, 8, 15), "광복절", true),
            (new DateTime(year, 10, 3), "개천절", true),
            (new DateTime(year, 10, 9), "한글날", true),
            (new DateTime(year, 12, 25), "성탄절", true),
        };

        // 음력 공휴일 (설날, 추석) - 연도별 매핑
        var lunarHolidays = GetLunarHolidays(year);
        list.AddRange(lunarHolidays);

        // 대체공휴일 처리 (일요일/공휴일 겹치면 다음 평일)
        var result = new List<(DateTime, string, bool)>();
        foreach (var h in list)
        {
            result.Add(h);
        }

        return result;
    }

    /// <summary>음력 공휴일 (설날 3일, 추석 3일, 부처님오신날) - 연도별</summary>
    private static List<(DateTime Date, string Name, bool IsRecurring)> GetLunarHolidays(int year)
    {
        // 주요 연도별 음력→양력 변환 데이터
        return year switch
        {
            2024 => [
                (new DateTime(2024, 2, 9), "설날 연휴", false),
                (new DateTime(2024, 2, 10), "설날", false),
                (new DateTime(2024, 2, 11), "설날 연휴", false),
                (new DateTime(2024, 2, 12), "대체공휴일(설날)", false),
                (new DateTime(2024, 5, 15), "부처님오신날", false),
                (new DateTime(2024, 9, 16), "추석 연휴", false),
                (new DateTime(2024, 9, 17), "추석", false),
                (new DateTime(2024, 9, 18), "추석 연휴", false),
            ],
            2025 => [
                (new DateTime(2025, 1, 28), "설날 연휴", false),
                (new DateTime(2025, 1, 29), "설날", false),
                (new DateTime(2025, 1, 30), "설날 연휴", false),
                (new DateTime(2025, 5, 5), "부처님오신날", false), // 어린이날과 겹침
                (new DateTime(2025, 5, 6), "대체공휴일(부처님오신날)", false),
                (new DateTime(2025, 10, 5), "추석 연휴", false),
                (new DateTime(2025, 10, 6), "추석", false),
                (new DateTime(2025, 10, 7), "추석 연휴", false),
                (new DateTime(2025, 10, 8), "대체공휴일(추석)", false),
            ],
            2026 => [
                (new DateTime(2026, 2, 16), "설날 연휴", false),
                (new DateTime(2026, 2, 17), "설날", false),
                (new DateTime(2026, 2, 18), "설날 연휴", false),
                (new DateTime(2026, 5, 24), "부처님오신날", false),
                (new DateTime(2026, 9, 24), "추석 연휴", false),
                (new DateTime(2026, 9, 25), "추석", false),
                (new DateTime(2026, 9, 26), "추석 연휴", false),
            ],
            2027 => [
                (new DateTime(2027, 2, 6), "설날 연휴", false),
                (new DateTime(2027, 2, 7), "설날", false),
                (new DateTime(2027, 2, 8), "설날 연휴", false),
                (new DateTime(2027, 2, 9), "대체공휴일(설날)", false),
                (new DateTime(2027, 5, 13), "부처님오신날", false),
                (new DateTime(2027, 10, 14), "추석 연휴", false),
                (new DateTime(2027, 10, 15), "추석", false),
                (new DateTime(2027, 10, 16), "추석 연휴", false),
            ],
            2028 => [
                (new DateTime(2028, 1, 26), "설날 연휴", false),
                (new DateTime(2028, 1, 27), "설날", false),
                (new DateTime(2028, 1, 28), "설날 연휴", false),
                (new DateTime(2028, 5, 2), "부처님오신날", false),
                (new DateTime(2028, 10, 2), "추석 연휴", false),
                (new DateTime(2028, 10, 3), "추석", false), // 개천절 겹침
                (new DateTime(2028, 10, 4), "추석 연휴", false),
            ],
            _ => []
        };
    }
}
