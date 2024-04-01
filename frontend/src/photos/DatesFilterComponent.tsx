// @flow
import * as React from 'react';
import { Box, Chip, Divider, Stack } from '@mui/material';
import { useEffect, useState } from 'react';
import { dateFromUnixTime, dateToUnixTime } from '../helpers/date-helper';

const minYear = 2000;

const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

type Props = {
  currentDate?: number;
  newDateSelected: (newDate: number) => void;
};

export function DatesFilterComponent({ currentDate, newDateSelected }: Readonly<Props>) {
  const [years, setYears] = useState<number[]>([]);
  const [currentYear, setCurrentYear] = useState<number>(0);
  const [currentMonth, setCurrentMonth] = useState<number>(0);

  useEffect(() => {
    const currentDate = new Date();
    const nowYear = currentDate.getFullYear();
    const nowMonth = currentDate.getMonth();
    const yearsArray: number[] = [];
    for (let i = nowYear; i >= minYear; i--) {
      yearsArray.push(i);
    }

    setYears(yearsArray);
    setCurrentYear(nowYear);
    setCurrentMonth(nowMonth);
  }, []);

  useEffect(() => {
    if (currentDate) {
      const date = dateFromUnixTime(currentDate);
      setCurrentYear(date.getFullYear());
      setCurrentMonth(date.getMonth());
    }
  }, [currentDate]);

  const handleYearChipClick = (year: number) => {
    const newDate = new Date(year, 12, 0);
    setCurrentYear(year);
    setCurrentMonth(0);
    newDateSelected(dateToUnixTime(newDate));
  };
  const handleMonthChipClick = (month: number) => {
    const newDate = new Date(currentYear, month, 1);
    setCurrentMonth(month);
    newDateSelected(dateToUnixTime(newDate));
  };

  return (
    <Box>
      <Stack
        direction="row"
        spacing={1}
        sx={{ marginTop: '10px', marginBottom: '10px' }}
        useFlexGap
        flexWrap="wrap"
      >
        {years.map((year) => {
          return (
            <Chip
              key={`year-${year}`}
              label={year.toString()}
              size="small"
              onClick={() => handleYearChipClick(year)}
              variant={currentYear === year ? 'filled' : 'outlined'}
            />
          );
        })}
      </Stack>
      <Divider />
      <Stack
        direction="row"
        spacing={1}
        sx={{ marginTop: '10px', marginBottom: '10px' }}
        useFlexGap
        flexWrap="wrap"
      >
        {months.map((m, index) => {
          return (
            <Chip
              key={`month-${m}`}
              label={m}
              size="small"
              onClick={() => handleMonthChipClick(index)}
              variant={currentMonth === index ? 'filled' : 'outlined'}
            />
          );
        })}
      </Stack>
    </Box>
  );
}
