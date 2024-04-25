// @flow
import * as React from 'react';
import { useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { MediaGeoLocationRegionSummary } from '../model/media-geo-location-region-summary';
import { backendAPI } from '../api/BackendApi';
import { Alert, Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { SimpleTreeView } from '@mui/x-tree-view/SimpleTreeView';
import { TreeItem } from '@mui/x-tree-view/TreeItem';
import { GeoLocationRegionPhotos } from './GeoLocationRegionPhotos';
import PlaceIcon from '@mui/icons-material/Place';

export function GeoLocationRegionPage() {
  const monthNames = [
    'Jan',
    'Feb',
    'Mar',
    'Apr',
    'May',
    'Jun',
    'Jul',
    'Aug',
    'Sep',
    'Oct',
    'Nov',
    'Dec',
  ];
  const { region } = useParams();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [regionSummary, setRegionSummary] = useState<MediaGeoLocationRegionSummary[]>([]);
  const [yearsSet, setYearsSet] = useState<number[]>([]);
  const [selectedMonth, setSelectedMonth] = useState<{ year: number; month: number } | undefined>(
    undefined,
  );

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    if (region) {
      backendAPI
        .getGeoLocationRegionSummary(region, authToken)
        .then((regionInfo) => {
          setIsLoading(false);
          setError(undefined);
          setRegionSummary(regionInfo);
          const years = [...new Set(regionInfo.map((r) => r.yearPart))];
          setYearsSet(years);
        })
        .catch((err) => {
          setIsLoading(false);
          setRegionSummary([]);
          setError(err.message);
        });
    }
  }, [region, authToken]);

  return (
    <Box
      sx={{
        marginTop: '10px',
        marginBottom: '10px',
        flexGrow: 1,
        display: 'flex',
        flexWrap: 'wrap',
        gap: '10px',
        rowGap: '10px',
        justifyContent: 'space-evenly',
        alignItems: 'center',
      }}
    >
      {isLoading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}
      {!isLoading && !error && regionSummary.length > 0 && (
        <Box sx={{ width: '100%', height: '100%', display: 'flex' }}>
          <SimpleTreeView
            aria-label="file system navigator"
            sx={{ height: '100%', flexGrow: 1, maxWidth: 400, minWidth: 200, overflowY: 'auto' }}
          >
            <h4>
              <Box sx={{ display: 'flex', gap: '2px', alignItems: 'center' }}>
                <PlaceIcon fontSize="small" />
                {region}
              </Box>
            </h4>
            {yearsSet.map((year) => (
              <TreeItem key={`tree-item-${year}`} itemId={year.toString()} label={year.toString()}>
                {regionSummary
                  .filter((r) => r.yearPart === year)
                  .map((r) => (
                    <TreeItem
                      key={`tree-item-${r.monthly}`}
                      itemId={r.monthly}
                      label={`${monthNames[r.monthPart - 1]} (${r.mediasCount} media)`}
                      onClick={() => setSelectedMonth({ year, month: r.monthPart })}
                    />
                  ))}
              </TreeItem>
            ))}
          </SimpleTreeView>
          <Box sx={{ height: '100%', flexGrow: 2 }}>
            <GeoLocationRegionPhotos region={region} monthly={selectedMonth} />
          </Box>
        </Box>
      )}
    </Box>
  );
}
