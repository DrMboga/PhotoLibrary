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

export function GeoLocationRegionPage() {
  const { region } = useParams();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [regionSummary, setRegionSummary] = useState<MediaGeoLocationRegionSummary[]>([]);

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    if (region) {
      backendAPI
        .getGeoLocationRegionSummary(region, authToken)
        .then((regionInfo) => {
          setIsLoading(false);
          setError(undefined);
          setRegionSummary(regionInfo);
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
        <Box>
          {/*{regionSummary.map((r) => (*/}
          {/*  <p key={r.monthly}>*/}
          {/*    {r.monthly}: {r.mediasCount}*/}
          {/*  </p>*/}
          {/*))}*/}
          <SimpleTreeView
            aria-label="file system navigator"
            sx={{ height: 200, flexGrow: 1, maxWidth: 400, overflowY: 'auto' }}
          >
            <TreeItem itemId="1" label="Applications">
              <TreeItem itemId="2" label="Calendar" />
            </TreeItem>
            <TreeItem itemId="5" label="Documents">
              <TreeItem itemId="10" label="OSS" />
              <TreeItem itemId="6" label="MUI">
                <TreeItem itemId="8" label="index.js" />
              </TreeItem>
            </TreeItem>
          </SimpleTreeView>
        </Box>
      )}
    </Box>
  );
}
