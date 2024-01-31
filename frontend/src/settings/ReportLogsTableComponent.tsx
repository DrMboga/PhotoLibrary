// @flow
import * as React from 'react';
import { ImportStepReport, ImportStepReportSeverity } from '../model/media-info';
import ReportIcon from '@mui/icons-material/Report';
import WarningIcon from '@mui/icons-material/Warning';
import InfoIcon from '@mui/icons-material/Info';
import { Box, Divider, Typography } from '@mui/material';
import { dateFromUnixTime } from '../helpers/date-helper';

type Props = {
  importerSteps: ImportStepReport[] | undefined;
};

export const ReportLogsTableComponent = ({ importerSteps }: Props) => {
  const getIcon = (status: ImportStepReportSeverity, id: string) => {
    switch (status) {
      case ImportStepReportSeverity.ERROR:
        return <ReportIcon fontSize="small" sx={{ color: 'red' }} id={`icon-status-${id}`} />;
      case ImportStepReportSeverity.WARNING:
        return <WarningIcon fontSize="small" sx={{ color: 'yellow' }} id={`icon-status-${id}`} />;
      case ImportStepReportSeverity.INFORMATION:
        return <InfoIcon fontSize="small" sx={{ color: 'green' }} id={`icon-status-${id}`} />;
    }
  };

  return (
    <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px', marginTop: '15px' }}>
      {importerSteps?.map((step) => (
        <Box key={`section-${step.id}`}>
          <Box
            id={`log-row-${step.id}`}
            sx={{
              display: 'flex',
              gap: '10px',
              alignItems: 'center',
              justifyContent: 'center',
            }}
          >
            <Typography id={`typography-1-${step.id}`} variant="body2">
              [ {dateFromUnixTime(step.timestamp).toLocaleString('ru-RU')} ]
            </Typography>
            {getIcon(step.severity, step.id)}
            <Typography
              id={`typography-2-${step.id}`}
              variant="body1"
              align="left"
              sx={{ width: '80%' }}
            >
              {step.stepMessage}
            </Typography>
          </Box>
          <Divider flexItem variant="middle" id={`divider-${step.id}`} />
        </Box>
      ))}
    </Box>
  );
};
